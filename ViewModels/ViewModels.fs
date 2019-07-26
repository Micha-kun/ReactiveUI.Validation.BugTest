namespace ViewModels

open ReactiveUI
open ReactiveUI.Validation.Extensions
open ReactiveUI.Validation.Contexts
open ReactiveUI.Validation.Abstractions
open System.Runtime.InteropServices
open System
open System.Reactive.Disposables
open Microsoft.FSharp.Linq.RuntimeHelpers
open System.Linq.Expressions

[<RequireQualifiedAccess>]
module Lambda =
    let toExpression (``f# lambda`` : Quotations.Expr<'a>) =
        ``f# lambda``
        |> LeafExpressionConverter.QuotationToExpression
        |> unbox<Expression<'a>>

[<AbstractClass>]
[<AllowNullLiteral>]
type ActivableReactiveObject() =
    inherit ReactiveObject()
    interface ISupportsActivation with
        member val Activator = new ViewModelActivator()

[<AbstractClass>]
type ViewModelBase(?mainThreadScheduler, ?taskPoolScheduler) =
    inherit ActivableReactiveObject()
    member this.TaskPoolScheduler = defaultArg taskPoolScheduler RxApp.TaskpoolScheduler
    member this.MainThreadScheduler = defaultArg mainThreadScheduler RxApp.MainThreadScheduler

[<AbstractClass>]
type ValidatableViewModelBase(?mainThreadScheduler, ?taskPoolScheduler) =
    inherit ViewModelBase(?mainThreadScheduler = mainThreadScheduler, ?taskPoolScheduler = taskPoolScheduler)
    let validationContext = new ValidationContext()

    interface ISupportsValidation with
        member this.ValidationContext with get () = validationContext

[<Sealed>]
type MainViewModel([<Optional; DefaultParameterValue(null)>] ?mainThreadScheduler, [<Optional; DefaultParameterValue(null)>] ?taskPoolScheduler) as this =
    inherit ValidatableViewModelBase(?mainThreadScheduler = mainThreadScheduler, ?taskPoolScheduler = taskPoolScheduler)
    let cif = ref ""
    
    do
        this.WhenActivated(fun (d: CompositeDisposable) ->
            this.ValidationRule(viewModelProperty = (<@ Func<MainViewModel, _>(fun vm -> vm.Cif) @> |> Lambda.toExpression),
                                isPropertyValid = Func<_,_>(fun cif -> String.IsNullOrWhiteSpace(cif)),
                                message = "Error").DisposeWith(d) |> ignore
        )

    member this.Cif
           with get () = !cif
           and set value = this.RaiseAndSetIfChanged(cif, value) |> ignore
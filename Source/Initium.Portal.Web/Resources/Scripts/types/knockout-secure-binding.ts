interface KnockoutSecureBindingOptions {
    attribute?: string;
    globals?: any;
    bindings: KnockoutBindingHandlers;
    noVirtualElements?: boolean;
}

interface KnockoutSecureBindingProvider extends KnockoutBindingProvider {
    new (options?: KnockoutSecureBindingOptions): KnockoutBindingProvider;
}
const
    path = require('path'),
    webpack = require('webpack'),
    MiniCssExtractPlugin = require('mini-css-extract-plugin'),
    CopyWebpackPlugin = require('copy-webpack-plugin'),
    {CleanWebpackPlugin} = require('clean-webpack-plugin'),
    CompressionPlugin = require('compression-webpack-plugin');

module.exports = {
    entry: {
        theme: path.resolve(__dirname, './Resources/Styles/theme.scss'),
        'vendors-styles': path.resolve(__dirname, './Resources/Styles/vendors.scss'),
        app: [path.resolve(__dirname, './Resources/Scripts/app.ts'), path.resolve(__dirname, './Resources/Styles/app.scss')],
        'users-list': path.resolve(__dirname, '../Initium.Portal.Web/Resources/Scripts/pages/users-list/users-list.ts'),
        'user-view': path.resolve(__dirname, '../Initium.Portal.Web/Resources/Scripts/pages/user-view/user-view.ts'),
        'role-create': path.resolve(__dirname, '../Initium.Portal.Web/Resources/Scripts/pages/role-create/role-create.ts'),
        'role-edit': path.resolve(__dirname, '../Initium.Portal.Web/Resources/Scripts/pages/role-edit/role-edit.ts'),
        'role-listing': path.resolve(__dirname, '../Initium.Portal.Web/Resources/Scripts/pages/role-list/role-list.ts'),
        'role-view': path.resolve(__dirname, '../Initium.Portal.Web/Resources/Scripts/pages/role-view/role-view.ts'),
        'profile-device': path.resolve(__dirname, '../Initium.Portal.Web/Resources/Scripts/pages/profile-device/profile-device.ts'),
        'profile-app': path.resolve(__dirname, '../Initium.Portal.Web/Resources/Scripts/pages/profile-app/profile-app.ts'),
        'validate-device-mfa': path.resolve(__dirname, '../Initium.Portal.Web/Resources/Scripts/pages/validate-device-mfa/validate-device-mfa.ts'),
        'validate-email-mfa': path.resolve(__dirname, '../Initium.Portal.Web/Resources/Scripts/pages/validate-email-mfa/validate-email-mfa.ts'),
        'notification-list': path.resolve(__dirname, '../Initium.Portal.Web/Resources/Scripts/pages/notification-list/notification-list.ts'),
        'system-alert-list': path.resolve(__dirname, '../Initium.Portal.Web/Resources/Scripts/pages/system-alert-list/system-alert-list.ts'),
        'tenant-create': path.resolve(__dirname, './Resources/Scripts/pages/tenant-create/tenant-create.ts'),
        'tenant-list': path.resolve(__dirname, './Resources/Scripts/pages/tenant-list/tenant-list.ts'),
        'tenant-view': path.resolve(__dirname, './Resources/Scripts/pages/tenant-view/tenant-view.ts'),

    },
    output: {
        filename: '[name].js',
        path: path.resolve(__dirname, 'wwwroot')
    },
    resolve: {
        extensions: ['.ts', '.tsx', '.js'],
        symlinks: false
    },
    module: {
        rules: [
            {
                test: /dataTables\.net.*/,
                use: 'imports-loader?define=>false,$=jquery'
            },
            {
                test: /\.tsx?$/,
                use: 'ts-loader'
            },
            {
                test: /\.(woff(2)?|ttf|eot|svg)(\?v=\d+\.\d+\.\d+)?$/,
                use: 'file-loader'
            },
            {
                test: /\.s[ac]ss$/i,
                use: [
                    MiniCssExtractPlugin.loader,
                    'css-loader',
                    'resolve-url-loader',
                    'sass-loader'
                ]
            },
            {
                test: /\.(png|svg|jpg|gif)$/,
                use: [
                    {
                        loader: 'url-loader',
                        options: {
                            limit: 1
                        }
                    }
                ]
            }
        ]
    },
    optimization: {
        splitChunks: {
            cacheGroups: {
                vendors: {
                    test: /[\\/]node_modules[\\/]/,
                    name: 'vendors',
                    chunks: 'all'
                },
                commons: {
                    name: 'commons',
                    test: /[\\/]Scripts[\\/](providers)[\\/]/,
                    chunks: 'all'
                },

            }
        }
    },
    plugins: [
        new CleanWebpackPlugin(),
        new MiniCssExtractPlugin({
            filename: '[name].css',
            chunkFilename: '[id].css',
            ignoreOrder: false
        }),
        new CopyWebpackPlugin({
            patterns: [
                path.resolve(__dirname, '../Initium.Portal.Web/Resources/Assets/logo.png'),
                path.resolve(__dirname, '../Initium.Portal.Web/Resources/Assets/logo-text.png'),
                path.resolve(__dirname, '../Initium.Portal.Web/Resources/Assets/logo-icon.png'),
                path.resolve(__dirname, '../Initium.Portal.Web/Resources/Assets/android-chrome-192x192.png'),
                path.resolve(__dirname, '../Initium.Portal.Web/Resources/Assets/android-chrome-512x512.png'),
                path.resolve(__dirname, '../Initium.Portal.Web/Resources/Assets/apple-touch-icon.png'),
                path.resolve(__dirname, '../Initium.Portal.Web/Resources/Assets/favicon.ico'),
                path.resolve(__dirname, '../Initium.Portal.Web/Resources/Assets/favicon-16x16.png'),
                path.resolve(__dirname, '../Initium.Portal.Web/Resources/Assets/favicon-32x32.png')
            ]}),
        new webpack.ProvidePlugin({
            $: "jquery",
            jQuery: "jquery",
            'window.jQuery': 'jquery',
            'window.$': 'jquery',
        }),
        new CompressionPlugin()
    ]
};
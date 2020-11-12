const 
    path = require('path'),
    webpack = require('webpack'),
    MiniCssExtractPlugin = require('mini-css-extract-plugin'),
    CopyWebpackPlugin = require('copy-webpack-plugin'),
    {CleanWebpackPlugin} = require('clean-webpack-plugin'),
    CompressionPlugin = require('compression-webpack-plugin');

module.exports = {
    entry: {
        theme: ['./Resources/Styles/theme.scss'],
        'vendors-styles': './Resources/Styles/vendors.scss',
        app: ['./Resources/Scripts/app.ts', './Resources/Styles/app.scss'],
        'users-list': './Resources/Scripts/core/pages/users-list/users-list.ts',
        'user-view': './Resources/Scripts/core/pages/user-view/user-view.ts',
        'role-create': './Resources/Scripts/core/pages/role-create/role-create.ts',
        'role-edit': './Resources/Scripts/core/pages/role-edit/role-edit.ts',
        'role-listing': './Resources/Scripts/core/pages/role-list/role-list.ts',
        'role-view': './Resources/Scripts/core/pages/role-view/role-view.ts',
        'profile-device': './Resources/Scripts/core/pages/profile-device/profile-device.ts',
        'profile-app': './Resources/Scripts/core/pages/profile-app/profile-app.ts',
        'validate-device-mfa': './Resources/Scripts/core/pages/validate-device-mfa/validate-device-mfa.ts',
        'validate-email-mfa': './Resources/Scripts/core/pages/validate-email-mfa/validate-email-mfa.ts',    
        'notification-list': './Resources/Scripts/core/pages/notification-list/notification-list.ts',    
        'system-alert-list': './Resources/Scripts/core/pages/system-alert-list/system-alert-list.ts',
            
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
                test: /theme\.s[ac]ss$/i,
                use: [
                    MiniCssExtractPlugin.loader,
                    'css-loader',                    
                    'sass-loader'
                ]
            }, 
            {
                test: /\.s[ac]ss$/i,
                exclude: /theme\.s[ac]ss$/i,
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
            './Resources/Assets/core/logo.png',
            './Resources/Assets/core/logo-text.png',
            './Resources/Assets/core/logo-icon.png',
            './Resources/Assets/core/android-chrome-192x192.png',
            './Resources/Assets/core/android-chrome-512x512.png',
            './Resources/Assets/core/apple-touch-icon.png',
            './Resources/Assets/core/favicon.ico',
            './Resources/Assets/core/favicon-16x16.png',
            './Resources/Assets/core/favicon-32x32.png'
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
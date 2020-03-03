const 
    path = require('path'),
    webpack = require('webpack'),
    MiniCssExtractPlugin = require('mini-css-extract-plugin'),
    {CleanWebpackPlugin} = require('clean-webpack-plugin');

module.exports = {
    entry: {
        theme: ['./Resources/Styles/theme.sass'],
        'vendors-styles': './Resources/Styles/vendors.sass',
        app: ['./Resources/Scripts/app.ts', './Resources/Styles/app.sass'],
        'users-list': './Resources/Scripts/pages/users-list.ts',
        'role-create': './Resources/Scripts/pages/role-create.ts',
        'role-edit': './Resources/Scripts/pages/role-edit.ts',
        'role-listing': './Resources/Scripts/pages/role-list.ts',
    },
    devtool: 'inline-source-map',
    output: {
        filename: '[name].js',
        path: path.resolve(__dirname, 'wwwroot')
    },
    resolve: {
        extensions: ['.ts', '.tsx', '.js']
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
                //include: [/node_modules/],

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
            // include all types of chunks
            cacheGroups: {
             vendors: {
                 test: /[\\/]node_modules[\\/]/,
                 name: 'vendors',
                 chunks: 'all'
               },
             commons: {
                 name: 'commons',
                 test: /[\\/]Scripts[\\/](services)[\\/]/,
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
        new webpack.ProvidePlugin({
            $: "jquery",
            jQuery: "jquery",
            'window.jQuery': 'jquery',
            'window.$': 'jquery',
        })
    ]
}
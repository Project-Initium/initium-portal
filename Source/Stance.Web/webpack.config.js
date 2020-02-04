const 
    path = require('path'),
    MiniCssExtractPlugin = require('mini-css-extract-plugin'),
    {CleanWebpackPlugin} = require('clean-webpack-plugin');

module.exports = {
    entry: {
        theme: ['./Resources/Styles/theme.sass'],
        app: ['./Resources/Styles/app.sass']
    },
    output: {
        filename: '[name].js',
        path: path.resolve(__dirname, 'wwwroot')
    },
    module: {
        rules: [
            {
                test: /\.s[ac]ss$/i,
                use: [
                    MiniCssExtractPlugin.loader,
                    'css-loader',
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
    plugins: [
        new CleanWebpackPlugin(),
        new MiniCssExtractPlugin({
            filename: '[name].css',
            chunkFilename: '[id].css',
            ignoreOrder: false
        })
    ]
}
const HtmlWebpackPlugin = require('html-webpack-plugin');
const MiniCssExtractPlugin = require('mini-css-extract-plugin');
const CopyWebpackPlugin = require('copy-webpack-plugin');
const path = require('path');

const entriesName = ['index'];

let entries = {};
/** @type { any[] } */
let plugins = [
    new MiniCssExtractPlugin(),
    new CopyWebpackPlugin({
        patterns: [
            {
                from: path.resolve(__dirname, 'public'),
                to: path.resolve(__dirname, 'dist'),
            },
        ],
    }),
];

for (let entry of entriesName) {
    entries[entry] = `./src/pages/${entry}.tsx`;
    plugins.push(
        new HtmlWebpackPlugin({
            template: './src/template.html',
            filename: `${entry}.html`,
            chunks: [entry],
        }),
    );
}

module.exports = {
    entry: entries,
    mode: 'development',
    devtool: 'source-map',
    stats: {
        warnings: false,
    },
    output: {
        filename: '[name].js',
        path: path.resolve(__dirname, 'dist'),
        clean: true,
    },
    module: {
        rules: [
            {
                test: /.(jsx?)|(tsx?)$/,
                exclude: /(node_modules)/,
                use: 'babel-loader',
            },
            {
                test: /\.css$/i,
                use: [MiniCssExtractPlugin.loader, 'css-loader', 'postcss-loader'],
            },
            {
                test: /\.(png|jpe?g|gif|svg|eot|ttf|woff|woff2)$/i,
                type: 'asset/resource',
            },
        ],
    },
    plugins: plugins,
    optimization: {
        splitChunks: {
            chunks: 'all',
        },
    },
    cache: {
        type: 'filesystem',
    },
    resolve: {
        extensions: ['.tsx', '.jsx', '.ts', '.js', '.wasm'],
    },
};

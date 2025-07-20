const webpackMerge = require('webpack-merge');
const baseConfig = require('./webpack.base.js');

module.exports = webpackMerge.merge(baseConfig, {
    mode: 'production',
    devtool: false,
});

module.exports = {
	globDirectory: 'build/deploy/',
	globPatterns: [
		'**/*.{ico,woff2,svg,html,json,js,png}'
	],
    globIgnores: ['**/remoteEntry.js'],
    swSrc: 'packages/asc-web-common/utils/sw-template.js',
	swDest: 'build/deploy/public/sw.js'
};
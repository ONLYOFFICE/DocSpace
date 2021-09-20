module.exports = {
	globDirectory: 'build/deploy/',
	globPatterns: [
		'**/*.{ico,woff2,svg,html,json,js,png}'
	],
	ignoreURLParametersMatching: [
		/^utm_/,
		/^fbclid$/
	],
	swDest: 'build/deploy/public/sw.js'
};
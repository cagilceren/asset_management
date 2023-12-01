import * as esbuild from 'esbuild'
import * as fs from  'fs'

let ctx = await esbuild.context({
	entryPoints: fs.readdirSync("js").filter(src => src.endsWith(".js")).map(x => "js/" + x).concat(['./css/deps.css']),
	bundle: true,
	outdir: './output/',
	plugins: [],
	loader: {
		".ttf": "file",
		".woff2": "file"
	},
	logLevel: "info", 
	minifyIdentifiers: false
});

if (process.argv[2] === "build") {
	console.log('[ESBUILD]: Rebuilding...', );
	let result = await ctx.rebuild()
	console.log('[ESBUILD]: done rebuilding...', result);

	ctx.dispose();
} else if (process.argv[2] === "watch") {
	console.log("[ESBUILD]: Watching...");
	await ctx.watch();
}

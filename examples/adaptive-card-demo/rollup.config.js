import resolve from '@rollup/plugin-node-resolve';
import commonjs from '@rollup/plugin-commonjs';



export default {
    input: 'index.js',
    external: ['dompurify'], // <-- suppresses the warning
    output: {
        file: 'dist/bundle.js',
        format: 'iife',
        name: 'card',
        
        globals: {
			dompurify: 'dompurify'
		}
    },
    plugins: [
        resolve(),
        commonjs()
    ]
};

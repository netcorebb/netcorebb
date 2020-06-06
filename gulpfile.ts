import {Gulpclass, Task} from "gulpclass/Decorators";

import * as gulp from "gulp";
import * as stylus from "gulp-stylus";
import * as rename from "gulp-rename";

const cleanCss = require("gulp-clean-css");


@Gulpclass()
export class Gulpfile {

    @Task("style")
    style() {
        return gulp.src([
           // "node_modules/normalize.css/normalize.css",
            "NetCoreBB.Admin/Styles/global.styl"
        ])
            .pipe(stylus({compress: false}))
            .pipe(cleanCss({debug: true}))
            .pipe(rename({
                basename: 'style',
                suffix: '.min',
            }))
            .pipe(gulp.dest("NetCoreBB.Admin/wwwroot"))
    }
}

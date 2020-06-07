import {Gulpclass, Task} from "gulpclass/Decorators";

import * as gulp from "gulp";
import * as stylus from "gulp-stylus";
import * as rename from "gulp-rename";

const cleanCss = require("gulp-clean-css");


@Gulpclass()
export class Gulpfile {

    @Task("style")
    style() {
        return gulp.src(
            "NetCoreBB.Admin/Styles/imports.styl"
        )
            .pipe(stylus({compress: false}))
            //.pipe(cleanCss({debug: false}))
            .pipe(rename("style.css"))
            .pipe(gulp.dest("NetCoreBB.Admin/wwwroot"))
    }
}

import {Gulpclass, SequenceTask, Task} from "gulpclass/Decorators";

import * as gulp from "gulp";
import {watch, series} from "gulp";
import * as stylus from "gulp-stylus";
import * as rename from "gulp-rename";
import * as inject from "gulp-inject";


//const cleanCss = require("gulp-clean-css");
const fs = require("fs");


@Gulpclass()
export class Gulpfile {

    readonly adminFolder = "NetCoreBB.Admin";
    readonly rootFolder = this.adminFolder + "/Root";
    readonly stylesFolder = this.adminFolder + "/Styles";


    @Task("style")
    style() {
        return gulp.src(this.stylesFolder + "/imports.styl")
            .pipe(stylus({compress: false}))
            //.pipe(cleanCss({debug: false}))
            .pipe(rename("style.css"))
            .pipe(gulp.dest(this.rootFolder, {overwrite: true}));
    }

    @Task("reload")
    reload() {
        return gulp.src(this.rootFolder + "/_reload.html")
            .pipe(inject(gulp.src(this.rootFolder + "/style.css"), {
                starttag: "<!-- inject -->",
                endtag: "<!-- endinject -->",
                removeTags: true,
                transform: (filePath) => {
                    const css = fs.readFileSync(filePath.substr(1)).toString();
                    return `<style>\n${css}</style>`;
                }
            }))
            .pipe(rename("reload.html"))
            .pipe(gulp.dest(this.rootFolder, {overwrite: true}));
    }

    @Task("watch")
    watch() {
        const tasks = series(["style", "reload"]);
        watch(this.adminFolder + "/**/*.styl", tasks);
    }

    @SequenceTask("dev")
    dev() {
        return ["style", "watch"];
    }
}

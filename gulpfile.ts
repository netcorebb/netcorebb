import {Gulpclass, Task} from "gulpclass/Decorators";

import * as gulp from "gulp";
import * as del from "del";

@Gulpclass()
export class Gulpfile {

    @Task()
    clean(cb: Function) {
        return del(["test"]);
    }
}

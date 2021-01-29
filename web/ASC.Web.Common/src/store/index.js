import { default as auth } from "./auth";
import ModuleStore from "./ModuleStore";

const moduleStore = new ModuleStore();

export default { auth, moduleStore };

import { default as auth } from "./auth";
import ModuleStore from "./ModuleStore";
import UserStore from "./UserStore";

const moduleStore = new ModuleStore();
const userStore = new UserStore();

export default { auth, moduleStore, userStore };

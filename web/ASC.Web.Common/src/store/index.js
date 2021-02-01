import { default as auth } from "./auth";
import ModuleStore from "./ModuleStore";
import UserStore from "./UserStore";
import SettingsStore from "./SettingsStore";

const moduleStore = new ModuleStore();
const userStore = new UserStore();
const settingsStore = new SettingsStore();

export default { auth, moduleStore, userStore, settingsStore };

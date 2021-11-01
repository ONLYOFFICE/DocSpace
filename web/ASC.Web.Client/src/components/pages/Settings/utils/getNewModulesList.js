export const getNewModulesList = (
  module,
  listAdminModules,
  availableModules
) => {
  const newModulesList = [];

  for (let i = 0; i < availableModules.length; i++) {
    if (availableModules[i].appName === module.appName) {
      newModulesList.push(availableModules[i].appName);
      continue;
    }

    for (let k = 0; k < listAdminModules.length; k++) {
      if (availableModules[i].appName === listAdminModules[k]) {
        newModulesList.push(availableModules[i].appName);
        break;
      }
    }
  }

  return newModulesList;
};



export const getCurrentSettingsCategory = (arrayOfParams, settingsTree) => {

    let key = "0-0";
    let CurrentSettingsGroupIndex = 0;
    let CurrentSettingsCategoryIndex = 0;

    let getCurrentSettingsGroupIndex = (currentParam) => {
        for(let i = 0; i<settingsTree.length; i++){
            if(currentParam && settingsTree[i].link === currentParam){
                return i
            }
        }
    }

    let getCurrentSettingsCategoryIndex = (currentParam) => {
        const currentCategories = settingsTree[CurrentSettingsGroupIndex].children;

        for(let i = 0; i<currentCategories.length; i++){
            if(currentParam && currentCategories[i].link === currentParam){
                return i
            }
        }
    }

    for(let i = 0; i<arrayOfParams.length || i<2; i++){

        const currentParam = arrayOfParams[i];

        if(currentParam){

            switch(i) {
                case 0 :
                    CurrentSettingsGroupIndex = getCurrentSettingsGroupIndex(currentParam);
                    key = CurrentSettingsGroupIndex !== undefined 
                        ? settingsTree[CurrentSettingsGroupIndex].key + "-0" 
                        : key
                    break
                
                case 1 :
                    CurrentSettingsCategoryIndex = getCurrentSettingsCategoryIndex(currentParam);
                    key = CurrentSettingsCategoryIndex !== undefined 
                        ? settingsTree[CurrentSettingsGroupIndex].children[CurrentSettingsCategoryIndex].key 
                        : key
                    break

                default : 
                    break
            }
        }
    }

    return key;
}
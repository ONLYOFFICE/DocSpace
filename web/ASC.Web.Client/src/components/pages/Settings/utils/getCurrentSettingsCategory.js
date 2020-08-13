

export const getCurrentSettingsCategory = (arrayOfParams, settingsTree) => {

    let key = "0-0";
    let groupIndex = 0;
    let categoryIndex = 0;

    const getCurrentSettingsGroupIndex = (currentParam) => {
        for(let i = 0; i<settingsTree.length; i++){
            if(currentParam && settingsTree[i].link === currentParam){
                return i
            }
        }
    }

    const getCurrentSettingsCategoryIndex = (currentParam) => {
        const currentCategories = settingsTree[groupIndex].children;

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
                    groupIndex = getCurrentSettingsGroupIndex(currentParam);
                    key = groupIndex !== undefined 
                        ? settingsTree[groupIndex].key + "-0" 
                        : key
                    break
                
                case 1 :
                    categoryIndex = getCurrentSettingsCategoryIndex(currentParam);
                    key = categoryIndex !== undefined 
                        ? settingsTree[groupIndex].children[categoryIndex].key 
                        : key
                    break

                default : 
                    break
            }
        }
    }

    return key;
}
import { store as commonStore } from 'asc-web-common';
import store from '../store/store';

const { getCurrentModule } = commonStore.auth.selectors;

export const setDocumentTitle = (subTitle = null ) => {
  const { auth: commonState } = store.getState();

  const { isAuthenticated, modules, settings } = commonState;
  const { organizationName, currentProductId } = settings;
 
  let title, currentModule; 

  if(modules && currentProductId ){
    currentModule = getCurrentModule(modules, currentProductId);
  }
  
  if (subTitle) {
    if (isAuthenticated && currentModule) {
      title = subTitle + ' - ' + currentModule.title;
    } else {
      title = subTitle + ' - ' + organizationName;
    }
  } else if (currentModule && organizationName) {
    title = currentModule.title + ' - ' + organizationName; 
  } else {
    title = organizationName;
  }

  document.title = title; 
}
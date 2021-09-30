import React from 'react';
import PropTypes from 'prop-types';
import { withRouter } from 'react-router';
import MainButton from '@appserver/components/main-button';
import DropDownItem from '@appserver/components/drop-down-item';
import { withTranslation } from 'react-i18next';
import { isMobile } from 'react-device-detect';
import { isMobile as isMobileUtils } from '@appserver/components/utils/device';
import Loaders from '@appserver/common/components/Loaders';
import { FileAction, AppServerConfig } from '@appserver/common/constants';
import { encryptionUploadDialog } from '../../../helpers/desktop';
import { inject, observer } from 'mobx-react';
import config from '../../../../package.json';
import { combineUrl } from '@appserver/common/utils';
import withLoader from '../../../HOCs/withLoader';

class CatalogMainButtonContent extends React.Component {
  onCreate = (e) => {
    // this.goToHomePage();
    const format = e.currentTarget.dataset.format || null;
    this.props.setAction({
      type: FileAction.Create,
      extension: format,
      id: -1,
    });
    if (isMobile || isMobileUtils()) this.props.toggleShowText();
  };

  onUploadFileClick = () => {
    if (this.props.isPrivacy) {
      encryptionUploadDialog((encryptedFile, encrypted) => {
        const { startUpload, t } = this.props;
        encryptedFile.encrypted = encrypted;
        this.goToHomePage();
        startUpload([encryptedFile], null, t);
      });
      if (isMobile || isMobileUtils()) this.props.toggleShowText();
    } else {
      this.inputFilesElement.click();
    }
  };

  onUploadFolderClick = () => this.inputFolderElement.click();

  goToHomePage = () => {
    const { homepage, history, filter } = this.props;
    const urlFilter = filter.toUrlParams();
    history.push(combineUrl(AppServerConfig.proxyURL, homepage, `/filter?${urlFilter}`));
    if (isMobile || isMobileUtils()) this.props.toggleShowText();
  };

  onFileChange = (e) => {
    const { startUpload, t } = this.props;
    //this.goToHomePage();
    startUpload(e.target.files, null, t);
    if (isMobile || isMobileUtils()) this.props.toggleShowText();
  };

  onInputClick = (e) => (e.target.value = null);

  // shouldComponentUpdate(nextProps, nextState) {
  //   return (
  //     nextProps.canCreate !== this.props.canCreate ||
  //     nextProps.firstLoad !== this.props.firstLoad ||
  //     nextProps.isPrivacy !== this.props.isPrivacy
  //   );
  // }

  render() {
    //console.log("Files ArticleMainButtonContent render");
    const { t, tReady, canCreate, isDisabled, firstLoad, isPrivacy } = this.props;

    return (
      <MainButton
        isDisabled={isDisabled ? isDisabled : !canCreate}
        isDropdown={true}
        text={t('Common:Actions')}>
        <DropDownItem
          className="main-button_drop-down"
          icon="images/actions.documents.react.svg"
          label={t('NewDocument')}
          onClick={this.onCreate}
          data-format="docx"
        />
        <DropDownItem
          className="main-button_drop-down"
          icon="images/spreadsheet.react.svg"
          label={t('NewSpreadsheet')}
          onClick={this.onCreate}
          data-format="xlsx"
        />
        <DropDownItem
          className="main-button_drop-down"
          icon="images/actions.presentation.react.svg"
          label={t('NewPresentation')}
          onClick={this.onCreate}
          data-format="pptx"
        />
        <DropDownItem
          className="main-button_drop-down"
          icon="images/catalog.folder.react.svg"
          label={t('NewFolder')}
          onClick={this.onCreate}
        />
        <DropDownItem isSeparator />
        <DropDownItem
          className="main-button_drop-down"
          icon="images/actions.upload.react.svg"
          label={t('UploadFiles')}
          onClick={this.onUploadFileClick}
        />
        {!isMobile && (
          <DropDownItem
            className="main-button_drop-down"
            icon="images/actions.upload.react.svg"
            label={t('UploadFolder')}
            disabled={isPrivacy}
            onClick={this.onUploadFolderClick}
          />
        )}
        <input
          id="customFileInput"
          className="custom-file-input"
          multiple
          type="file"
          onChange={this.onFileChange}
          onClick={this.onInputClick}
          ref={(input) => (this.inputFilesElement = input)}
          style={{ display: 'none' }}
        />
        <input
          id="customFolderInput"
          className="custom-file-input"
          webkitdirectory=""
          mozdirectory=""
          type="file"
          onChange={this.onFileChange}
          onClick={this.onInputClick}
          ref={(input) => (this.inputFolderElement = input)}
          style={{ display: 'none' }}
        />
      </MainButton>
    );
  }
}

CatalogMainButtonContent.propTypes = {
  history: PropTypes.object.isRequired,
};

export default inject(({ auth, filesStore, uploadDataStore, treeFoldersStore }) => {
  const { firstLoad, fileActionStore, filter, canCreate } = filesStore;
  const { isPrivacyFolder } = treeFoldersStore;
  const { startUpload } = uploadDataStore;

  return {
    homepage: config.homepage,
    firstLoad,
    isPrivacy: isPrivacyFolder,
    filter,
    canCreate,

    setAction: fileActionStore.setAction,
    startUpload,

    toggleShowText: auth.settingsStore.toggleShowText,
  };
})(
  withRouter(
    withTranslation(['Article', 'Common'])(
      withLoader(observer(CatalogMainButtonContent))(<Loaders.MainButton />),
    ),
  ),
);

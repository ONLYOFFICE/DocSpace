import React from 'react';
import { connect } from 'react-redux';
import PropTypes from 'prop-types';
import { withRouter } from 'react-router';
import { MainButton, DropDownItem } from "asc-web-components";
import { withTranslation, I18nextProvider } from "react-i18next";
import { setAction } from "../../../store/files/actions";
import { isCanCreate } from "../../../store/files/selectors";
import i18n from "../i18n";
import { utils as commonUtils, constants } from "asc-web-common";

const { changeLanguage } = commonUtils;
const { FileAction } = constants;

class PureArticleMainButtonContent extends React.Component {

  onCreate = (e) => {
    const format = e.currentTarget.dataset.format || null;
    this.props.setAction({
      type: FileAction.Create,
      extension: format,
      id: -1,
    });
  };

  onUploadFileClick = () => this.inputFilesElement.click();
  onUploadFolderClick = () => this.inputFolderElement.click();

  onFileChange = (e) => this.props.startUpload(e.target.files);
  onInputClick = e => e.target.value = null;

  shouldComponentUpdate(nextProps, nextState) {
    return nextProps.isCanCreate !== this.props.isCanCreate;
  }

  render() {
    //console.log("Files ArticleMainButtonContent render");
    const { t, isCanCreate } = this.props;

    return (
      <MainButton
        isDisabled={!isCanCreate}
        isDropdown={true}
        text={t('Actions')}
      >
        <DropDownItem
          icon="ActionsDocumentsIcon"
          label={t('NewDocument')}
          onClick={this.onCreate}
          data-format="docx"
        />
        <DropDownItem
          icon="SpreadsheetIcon"
          label={t('NewSpreadsheet')}
          onClick={this.onCreate}
          data-format="xlsx"
        />
        <DropDownItem
          icon="ActionsPresentationIcon"
          label={t('NewPresentation')}
          onClick={this.onCreate}
          data-format="pptx"
        />
        <DropDownItem
          icon="CatalogFolderIcon"
          label={t('NewFolder')}
          onClick={this.onCreate}
        />
        <DropDownItem isSeparator />
        <DropDownItem
          icon="ActionsUploadIcon"
          label={t("UploadFiles")}
          onClick={this.onUploadFileClick}
        />
        <DropDownItem
          icon="ActionsUploadIcon"
          label={t("UploadFolder")}
          onClick={this.onUploadFolderClick}
        />
        <input
          id="customFile"
          className="custom-file-input"
          multiple
          type="file"
          onChange={this.onFileChange}
          onClick={this.onInputClick}
          ref={(input) => (this.inputFilesElement = input)}
          style={{ display: "none" }}
        />
        <input
          id="customFile"
          className="custom-file-input"
          webkitdirectory=""
          mozdirectory=""
          type="file"
          onChange={this.onFileChange}
          onClick={this.onInputClick}
          ref={(input) => (this.inputFolderElement = input)}
          style={{ display: "none" }}
        />
      </MainButton>
    );
  }
}

const ArticleMainButtonContentContainer = withTranslation()(PureArticleMainButtonContent);

const ArticleMainButtonContent = (props) => {
  changeLanguage(i18n);
  return (<I18nextProvider i18n={i18n}><ArticleMainButtonContentContainer {...props} /></I18nextProvider>);
};

ArticleMainButtonContent.propTypes = {
  isAdmin: PropTypes.bool,
  history: PropTypes.object.isRequired
};

const mapStateToProps = (state) => {
  const { selectedFolder } = state.files;
  const { user } = state.auth;

  return {
    isCanCreate: isCanCreate(selectedFolder, user)
  };
};

export default connect(mapStateToProps, { setAction })(
  withRouter(ArticleMainButtonContent)
);

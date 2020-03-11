import React from 'react';
import { connect } from 'react-redux';
import PropTypes from 'prop-types';
import { withRouter } from 'react-router';
import {
  MainButton,
  DropDownItem,
  toastr
} from "asc-web-components";
import { withTranslation, I18nextProvider } from 'react-i18next';
import i18n from '../i18n';
import { store, utils } from 'asc-web-common';
const { changeLanguage } = utils;
const { isAdmin } = store.auth.selectors;

class PureArticleMainButtonContent extends React.Component {

  onCreate = (format) => {
    this.props.onCreate(format);
  }

  render() {
    console.log("Files ArticleMainButtonContent render");
    const { isAdmin, t } = this.props;

    return (
      isAdmin ?
        <>
          <MainButton
            isDisabled={false}
            isDropdown={true}
            text={t('Actions')}
          >
            <DropDownItem
              icon="ActionsDocumentsIcon"
              label={t('NewDocument')}
              onClick={this.onCreate.bind(this, 'docx')}
            />
            <DropDownItem
              icon="SpreadsheetIcon"
              label={t('NewSpreadsheet')}
              onClick={this.onCreate.bind(this, 'xlsx')}
            />
            <DropDownItem
              icon="ActionsPresentationIcon"
              label={t('NewPresentation')}
              onClick={this.onCreate.bind(this, 'pptx')}
            />
            <DropDownItem
              icon="CatalogFolderIcon"
              label={t('NewFolder')}
              onClick={this.onCreate.bind(this, 'folder')}
            />
            <DropDownItem isSeparator />
            <DropDownItem
              icon="ActionsUploadIcon"
              label={t('Upload')}
              onClick={() => toastr.info("Upload click")}
            />
          </MainButton>
        </>
        :
        <></>
    );
  };
};

const ArticleMainButtonContentContainer = withTranslation()(PureArticleMainButtonContent);

const ArticleMainButtonContent = (props) => {
  changeLanguage(i18n);
  return (<I18nextProvider i18n={i18n}><ArticleMainButtonContentContainer {...props} /></I18nextProvider>);
};

ArticleMainButtonContent.propTypes = {
  isAdmin: PropTypes.bool.isRequired,
  history: PropTypes.object.isRequired
};

const mapStateToProps = (state) => {
  return {
    isAdmin: isAdmin(state.auth.user),
    settings: state.auth.settings
  }
}

export default connect(mapStateToProps)(withRouter(ArticleMainButtonContent));
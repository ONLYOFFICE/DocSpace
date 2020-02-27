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
              icon="AddEmployeeIcon"
              label={"Document"}
              onClick={() => toastr.info("Create document click")}
            />
            <DropDownItem
              icon="AddDepartmentIcon"
              label={"Folder"}
              onClick={() => toastr.info("Create folder click")}
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
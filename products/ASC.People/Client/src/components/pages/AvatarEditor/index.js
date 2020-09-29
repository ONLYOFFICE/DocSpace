import React, {useEffect} from "react";
import { connect } from "react-redux";
// import PropTypes from "prop-types";
import { PageLayout, store, utils } from "asc-web-common";
import {
  ArticleHeaderContent,
  ArticleMainButtonContent,
  ArticleBodyContent
} from "../../Article";
// import { SectionHeaderContent } from './Section';
// import { fetchProfile } from '../../../store/profile/actions';
import { I18nextProvider, withTranslation } from "react-i18next";
import { SectionHeaderContent, SectionBodyContent } from "./Section";
import { createI18N } from "../../../helpers/i18n";
const { changeLanguage } = utils;
const i18n = createI18N({
  page: "AvatarEditor",
  localesPath: "pages/AvatarEditor"
});
const { isAdmin } = store.auth.selectors;

class AvatarEditor extends React.Component {
  componentDidMount() {
    changeLanguage(i18n);
  }

  render() {
    const { isAdmin } = this.props;
    console.log("AvatarEditor render");

    return (
      <I18nextProvider i18n={i18n}>
        <PageLayout>
          <PageLayout.ArticleHeader>
            <ArticleHeaderContent />
          </PageLayout.ArticleHeader>

          {isAdmin && (
            <PageLayout.ArticleMainButton>
              <ArticleMainButtonContent />
            </PageLayout.ArticleMainButton>
          )}

          <PageLayout.ArticleBody>
            <ArticleBodyContent />
          </PageLayout.ArticleBody>

          <PageLayout.SectionHeader>
            <SectionHeaderContent />
          </PageLayout.SectionHeader>

          <PageLayout.SectionBody>
            <SectionBodyContent />
          </PageLayout.SectionBody>
        </PageLayout>
      </I18nextProvider>
    );
  }
}


AvatarEditor.propTypes = {
  
};

const AvatarEditorWrapper = withTranslation()(AvatarEditor);

const AvatarEditorContainer = props => {
  useEffect(() => {
    changeLanguage(i18n);
  }, []);
  return (
    <I18nextProvider i18n={i18n}>
      <AvatarEditorWrapper {...props} />
    </I18nextProvider>
  );
};

function mapStateToProps(state) {
  return {
    settings: state.auth.settings,
    group: state.group.targetGroup,
    isAdmin: isAdmin(state.auth.user)
  };
}

export default connect(
  mapStateToProps,
  {}
)(AvatarEditorContainer);
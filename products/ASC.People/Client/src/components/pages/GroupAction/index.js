import React from "react";
import { connect } from "react-redux";
import { PageLayout } from "asc-web-components";
import { ArticleHeaderContent, ArticleMainButtonContent, ArticleBodyContent } from '../../Article';
import { SectionHeaderContent, SectionBodyContent } from './Section';
import i18n from "./i18n";
import { I18nextProvider } from "react-i18next";

class GroupAction extends React.Component {

  render() {
    console.log("GroupAction render")

    return (
      <I18nextProvider i18n={i18n}>
        <PageLayout
          articleHeaderContent={<ArticleHeaderContent />}
          articleMainButtonContent={<ArticleMainButtonContent />}
          articleBodyContent={<ArticleBodyContent />}
          sectionHeaderContent={<SectionHeaderContent />}
          sectionBodyContent={<SectionBodyContent />}
        />
      </I18nextProvider>
    );
  }
}

function mapStateToProps(state) {
  return {
    settings: state.auth.settings
  };
}

export default connect(mapStateToProps)(GroupAction);
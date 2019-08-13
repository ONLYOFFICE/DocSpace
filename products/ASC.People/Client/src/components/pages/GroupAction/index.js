import React from "react";
import { connect } from "react-redux";
import { PageLayout } from "asc-web-components";
import { ArticleHeaderContent, ArticleMainButtonContent, ArticleBodyContent } from '../../Article';
import { SectionHeaderContent, SectionBodyContent } from './Section';

class GroupAction extends React.Component {

  render() {
    console.log("GroupAction render")

    return (
      <PageLayout
          articleHeaderContent={<ArticleHeaderContent />}
          articleMainButtonContent={<ArticleMainButtonContent />}
          articleBodyContent={<ArticleBodyContent />}
          sectionHeaderContent={<SectionHeaderContent />}
          sectionBodyContent={<SectionBodyContent />}
        />
    );
  }
}

function mapStateToProps(state) {
  return {
    settings: state.auth.settings
  };
}

export default connect(mapStateToProps)(GroupAction);
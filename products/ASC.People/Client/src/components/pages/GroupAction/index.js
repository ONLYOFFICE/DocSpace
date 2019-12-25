import React from "react";
import { connect } from "react-redux";
import { Loader } from "asc-web-components";
import { PageLayout } from "asc-web-common";
import { ArticleHeaderContent, ArticleMainButtonContent, ArticleBodyContent } from '../../Article';
import { SectionHeaderContent, SectionBodyContent } from './Section';
import i18n from "./i18n";
import { I18nextProvider } from "react-i18next";
import { fetchGroup, resetGroup } from "../../../store/group/actions";

class GroupAction extends React.Component {

  componentDidMount() {
    const { match, fetchGroup } = this.props;
    const { groupId } = match.params;

    if (groupId) {
      fetchGroup(groupId);
    }
  }

  componentDidUpdate(prevProps) {
    const { match, fetchGroup, resetGroup } = this.props;
    const { groupId } = match.params;
    const prevUserId = prevProps.match.params.groupId;

    if (groupId !== prevUserId) {
      groupId ? fetchGroup(groupId) : resetGroup();
    }
  }

  render() {
    console.log("GroupAction render")

    const { group, match, language } = this.props;

    i18n.changeLanguage(language);

    return (
      <I18nextProvider i18n={i18n}>
        {group || !match.params.groupId
        ? <PageLayout
          withBodyScroll={true}
          articleHeaderContent={<ArticleHeaderContent />}
          articleMainButtonContent={<ArticleMainButtonContent />}
          articleBodyContent={<ArticleBodyContent />}
          sectionHeaderContent={<SectionHeaderContent />}
          sectionBodyContent={<SectionBodyContent />}
        />
        : <PageLayout
          articleHeaderContent={<ArticleHeaderContent />}
          articleMainButtonContent={<ArticleMainButtonContent />}
          articleBodyContent={<ArticleBodyContent />}
          sectionBodyContent={<Loader className="pageLoader" type="rombs" size='40px' />}
          />
        }
      </I18nextProvider>
    );
  }
}

function mapStateToProps(state) {
  return {
    settings: state.auth.settings,
    language: state.auth.user.cultureName || state.auth.settings.culture,
    group: state.group.targetGroup
  };
}

export default connect(mapStateToProps, { fetchGroup, resetGroup })(GroupAction);
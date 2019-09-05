import React from "react";
import { connect } from "react-redux";
import { PageLayout, Loader } from "asc-web-components";
import { ArticleHeaderContent, ArticleMainButtonContent, ArticleBodyContent } from '../../Article';
import { SectionHeaderContent, SectionBodyContent } from './Section';
import i18n from "./i18n";
import { I18nextProvider } from "react-i18next";
import { fetchGroup } from "../../../store/group/actions";

class GroupAction extends React.Component {

  componentDidMount() {
    const { match, fetchGroup } = this.props;
    const { groupId } = match.params;

    if (groupId) {
      fetchGroup(groupId);
    }
  }

  componentDidUpdate(prevProps) {
    const { match, fetchGroup } = this.props;
    const { groupId } = match.params;
    const prevUserId = prevProps.match.params.groupId;

    if (groupId !== undefined && groupId !== prevUserId) {
      fetchGroup(groupId);
    }
  }

  render() {
    console.log("GroupAction render")

    const { group, match } = this.props;

    return (
      <I18nextProvider i18n={i18n}>
        {group || !match.params.groupId
        ? <PageLayout
          articleHeaderContent={<ArticleHeaderContent />}
          articleMainButtonContent={<ArticleMainButtonContent />}
          articleBodyContent={<ArticleBodyContent />}
          sectionHeaderContent={<SectionHeaderContent />}
          sectionBodyContent={<SectionBodyContent group={group} />}
        />
        : <PageLayout
          articleHeaderContent={<ArticleHeaderContent />}
          articleMainButtonContent={<ArticleMainButtonContent />}
          articleBodyContent={<ArticleBodyContent />}
          sectionBodyContent={<Loader className="pageLoader" type="rombs" size={40} />}
          />
        }
      </I18nextProvider>
    );
  }
}

function mapStateToProps(state) {
  return {
    settings: state.auth.settings,
    group: state.group.targetGroup
  };
}

export default connect(mapStateToProps, { fetchGroup })(GroupAction);
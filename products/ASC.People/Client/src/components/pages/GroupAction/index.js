import React from "react";
import { connect } from "react-redux";
import { Loader } from "asc-web-components";
import { PageLayout, utils } from "asc-web-common";
import {
  ArticleHeaderContent,
  ArticleMainButtonContent,
  ArticleBodyContent
} from '../../Article';
import { SectionHeaderContent, SectionBodyContent } from './Section';
import i18n from "./i18n";
import { I18nextProvider, withTranslation } from "react-i18next";
import { fetchGroup, resetGroup } from "../../../store/group/actions";
const { changeLanguage } = utils;

class GroupAction extends React.Component {

  componentDidMount() {
    const { match, fetchGroup, t } = this.props;
    const { groupId } = match.params;

    document.title = `${t("GroupAction")} – ${t("People")}`;

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

    const { group, match } = this.props;

    changeLanguage(i18n);

    return (
      <I18nextProvider i18n={i18n}>
        {group || !match.params.groupId ? (
          <PageLayout withBodyScroll={true}>
            <PageLayout.ArticleHeader>
              <ArticleHeaderContent />
            </PageLayout.ArticleHeader>

            <PageLayout.ArticleMainButton>
              <ArticleMainButtonContent />
            </PageLayout.ArticleMainButton>

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
        ) : (
            <PageLayout>
              <PageLayout.ArticleHeader>
                <ArticleHeaderContent />
              </PageLayout.ArticleHeader>

              <PageLayout.ArticleMainButton>
                <ArticleMainButtonContent />
              </PageLayout.ArticleMainButton>

              <PageLayout.ArticleBody>
                <ArticleBodyContent />
              </PageLayout.ArticleBody>

              <PageLayout.SectionBody>
                <Loader className="pageLoader" type="rombs" size="40px" />
              </PageLayout.SectionBody>
            </PageLayout>
          )}
      </I18nextProvider>
    );
  }
}

const GroupActionWrapper = withTranslation()(GroupAction);

const GroupActionContainer = props => {
  changeLanguage(i18n);
  return (
    <I18nextProvider i18n={i18n}>
      <GroupActionWrapper {...props} />
    </I18nextProvider>
  );
};

function mapStateToProps(state) {
  return {
    settings: state.auth.settings,
    group: state.group.targetGroup
  };
}

export default connect(mapStateToProps,
  {
    fetchGroup,
    resetGroup
  })(GroupActionContainer);
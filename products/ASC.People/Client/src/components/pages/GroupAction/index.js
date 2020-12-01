import React, { useEffect } from "react";
import { connect } from "react-redux";
import { Loader } from "asc-web-components";
import { PageLayout, utils, store } from "asc-web-common";
import {
  ArticleHeaderContent,
  ArticleMainButtonContent,
  ArticleBodyContent,
} from "../../Article";
import { SectionHeaderContent, SectionBodyContent } from "./Section";
import { I18nextProvider, withTranslation } from "react-i18next";
import { fetchGroup, resetGroup } from "../../../store/group/actions";
import { createI18N } from "../../../helpers/i18n";
import { setDocumentTitle } from "../../../helpers/utils";
import { withRouter } from "react-router";
const i18n = createI18N({
  page: "GroupAction",
  localesPath: "pages/GroupAction",
});
const { changeLanguage } = utils;
const { isAdmin } = store.auth.selectors;

class GroupAction extends React.Component {
  componentDidMount() {
    const { match, fetchGroup, t } = this.props;
    const { groupId } = match.params;

    setDocumentTitle(t("GroupAction"));
    changeLanguage(i18n);

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
    console.log("GroupAction render");

    const { group, match, isAdmin } = this.props;

    return (
      <I18nextProvider i18n={i18n}>
        {group || !match.params.groupId ? (
          <PageLayout withBodyScroll={true}>
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
        ) : (
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

            <PageLayout.SectionBody>
              <Loader className="pageLoader" type="rombs" size="40px" />
            </PageLayout.SectionBody>
          </PageLayout>
        )}
      </I18nextProvider>
    );
  }
}

const GroupActionWrapper = withTranslation()(withRouter(GroupAction));

const GroupActionContainer = (props) => {
  useEffect(() => {
    changeLanguage(i18n);
  }, []);
  return (
    <I18nextProvider i18n={i18n}>
      <GroupActionWrapper {...props} />
    </I18nextProvider>
  );
};

function mapStateToProps(state) {
  return {
    settings: state.auth.settings,
    group: state.group.targetGroup,
    isAdmin: isAdmin(state),
  };
}

export default connect(mapStateToProps, {
  fetchGroup,
  resetGroup,
})(GroupActionContainer);

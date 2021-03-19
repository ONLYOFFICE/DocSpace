import React from "react";
import { Loader } from "asc-web-components";
import { PageLayout } from "asc-web-common";
import {
  ArticleHeaderContent,
  ArticleMainButtonContent,
  ArticleBodyContent,
} from "../../Article";
import { SectionHeaderContent, SectionBodyContent } from "./Section";
import { withTranslation } from "react-i18next";
import { withRouter } from "react-router";
import { inject, observer } from "mobx-react";

class GroupAction extends React.Component {
  componentDidMount() {
    const { match, fetchGroup, t, setDocumentTitle } = this.props;
    const { groupId } = match.params;

    setDocumentTitle(t("GroupAction"));

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

  componentWillUnmount() {
    this.props.resetGroup();
  }

  render() {
    console.log("GroupAction render");

    const { group, match, isAdmin } = this.props;

    return (
      <>
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
      </>
    );
  }
}

const GroupActionContainer = withTranslation("GroupAction")(
  withRouter(GroupAction)
);

export default inject(({ auth, peopleStore }) => ({
  setDocumentTitle: auth.setDocumentTitle,
  isAdmin: auth.isAdmin,
  fetchGroup: peopleStore.selectedGroupStore.setTargetedGroup,
  group: peopleStore.selectedGroupStore.targetedGroup,
  resetGroup: peopleStore.selectedGroupStore.resetGroup,
}))(observer(GroupActionContainer));

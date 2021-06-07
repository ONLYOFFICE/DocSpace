import React from "react";
import Loader from "@appserver/components/loader";
import PageLayout from "@appserver/common/components/PageLayout";
import {
  ArticleHeaderContent,
  ArticleMainButtonContent,
  ArticleBodyContent,
} from "../../components/Article";
import { SectionHeaderContent, SectionBodyContent } from "./Section";
import { withTranslation } from "react-i18next";
import { withRouter } from "react-router";
import { inject, observer } from "mobx-react";

class GroupAction extends React.Component {
  componentDidMount() {
    const { match, fetchGroup, t, setDocumentTitle, setFirstLoad } = this.props;
    const { groupId } = match.params;
    setFirstLoad(false);
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

    const { group, match, tReady } = this.props;

    return (
      <>
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
              <SectionBodyContent tReady={tReady} />
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
      </>
    );
  }
}

const GroupActionContainer = withTranslation(["GroupAction", "Common"])(
  GroupAction
);
export default withRouter(
  inject(({ auth, peopleStore }) => {
    const { setDocumentTitle } = auth;
    const { selectedGroupStore, loadingStore } = peopleStore;
    const {
      setTargetedGroup: fetchGroup,
      targetedGroup: group,
      resetGroup,
    } = selectedGroupStore;
    const { setFirstLoad } = loadingStore;
    return {
      setDocumentTitle,
      fetchGroup,
      group,
      resetGroup,
      setFirstLoad,
    };
  })(observer(GroupActionContainer))
);

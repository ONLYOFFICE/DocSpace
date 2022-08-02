import React from "react";
import Loader from "@docspace/components/loader";
import Section from "@docspace/common/components/Section";

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
    // console.log("GroupAction render");

    const { group, match, tReady, isAdmin, showCatalog } = this.props;

    return (
      <>
        {group || !match.params.groupId ? (
          <Section withBodyScroll={true}>
            <Section.SectionHeader>
              <SectionHeaderContent />
            </Section.SectionHeader>

            <Section.SectionBody>
              <SectionBodyContent tReady={tReady} />
            </Section.SectionBody>
          </Section>
        ) : (
          <Section>
            <Section.SectionBody>
              <Loader className="pageLoader" type="rombs" size="40px" />
            </Section.SectionBody>
          </Section>
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
      isAdmin: auth.isAdmin,
      showCatalog: auth.settingsStore.showCatalog,
    };
  })(observer(GroupActionContainer))
);

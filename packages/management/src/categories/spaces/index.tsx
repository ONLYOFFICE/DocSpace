import React from "react";
import { useTranslation } from "react-i18next";

import Text from "@docspace/components/text";
import MultipleSpaces from "./sub-components/MultipleSpaces";
import { SpaceContainer } from "./StyledSpaces";
import ConfigurationSection from "./sub-components/ConfigurationSection";
import { observer } from "mobx-react";
import { useStore } from "SRC_DIR/store";
import ChangeDomainDialog from "./sub-components/dialogs/ChangeDomainDialog";
import CreatePortalDialog from "./sub-components/dialogs/CreatePortalDialog";

const Spaces = () => {
  const { t } = useTranslation(["Common"]);

  const [isLoading, setIsLoading] = React.useState<boolean>(true);

  const { spacesStore, authStore } = useStore();

  const {
    initStore,
    isConnected,
    portals,
    domainDialogVisible,
    createPortalDialogVisible,
  } = spacesStore;
  const { setDocumentTitle } = authStore;

  React.useEffect(() => {
    const fetchData = async () => {
      await initStore();
      setIsLoading(false);
    };

    fetchData();
    setDocumentTitle(t("Common:Spaces"));
  }, []);

  if (isLoading) return <h1>Loading</h1>;

  return (
    <SpaceContainer>
      {domainDialogVisible && <ChangeDomainDialog key="change-domain-dialog" />}
      {createPortalDialogVisible && (
        <CreatePortalDialog key="create-portal-dialog" />
      )}
      <div className="spaces-header">
        <Text>
          Use this section to create several spaces and make them accessible for
          your users
        </Text>
      </div>
      {isConnected && portals.length > 0 ? (
        <MultipleSpaces />
      ) : (
        <ConfigurationSection />
      )}
    </SpaceContainer>
  );
};

export default observer(Spaces);

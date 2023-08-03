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
  const [isLoading, setIsLoading] = React.useState<boolean>(true);

  const { t } = useTranslation(["Management", "Common"]);

  const { spacesStore } = useStore();

  const {
    initStore,
    isConnected,
    portals,
    domainDialogVisible,
    createPortalDialogVisible,
  } = spacesStore;

  React.useEffect(() => {
    const fetchData = async () => {
      await initStore();
      setIsLoading(false);
    };

    fetchData();
  }, []);

  if (isLoading) return <h1>Loading</h1>;

  return (
    <SpaceContainer>
      {domainDialogVisible && <ChangeDomainDialog key="change-domain-dialog" />}
      {createPortalDialogVisible && (
        <CreatePortalDialog key="create-portal-dialog" />
      )}
      <div className="spaces-header">
        <Text>{t("Subheader")}</Text>
      </div>
      {isConnected && portals.length > 0 ? (
        <MultipleSpaces />
      ) : (
        <ConfigurationSection t={t} />
      )}
    </SpaceContainer>
  );
};

export default observer(Spaces);

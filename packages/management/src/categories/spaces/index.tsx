import React from "react";
import Text from "@docspace/components/text";
import MultipleSpaces from "./sub-components/MultipleSpaces";
import { SpaceContainer } from "./StyledSpaces";
import ConfigurationSection from "./sub-components/ConfigurationSection";
import { observer } from "mobx-react";
import { useStore } from "SRC_DIR/store";

const Spaces = () => {
  const [data, setData] = React.useState<Array<object>>([]);

  const { spacesStore, authStore } = useStore();

  const { getAllPortals, getPortalDomain, isConnected, portals } = spacesStore;

  React.useEffect(() => {
    getPortalDomain();
    getAllPortals();
  }, []);

  return (
    <SpaceContainer>
      <div className="spaces-header">
        <Text>
          Use this section to create several spaces and make them accessible for
          your users
        </Text>
      </div>
      {isConnected && portals.length > 0 ? (
        <MultipleSpaces portals={portals} />
      ) : (
        <ConfigurationSection setData={setData} />
      )}
    </SpaceContainer>
  );
};

export default observer(Spaces);

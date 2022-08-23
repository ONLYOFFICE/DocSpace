import React from "react";
import styled from "styled-components";
import { isMobile } from "react-device-detect";

import RowContainer from "@docspace/components/row-container";
import RowContent from "@docspace/components/row-content";
import Row from "@docspace/components/row";

import Text from "@docspace/components/text";

import { tablet } from "@docspace/components/utils/device";
import { Base } from "@docspace/components/themes";

import { activatePlugin, deletePlugin } from "SRC_DIR/helpers/plugins";

const StyledHeader = styled.div`
  display: ${isMobile ? "none" : "flex"};
  border-bottom: ${(props) => props.theme.connectedClouds.borderBottom};
  padding-bottom: 12px;

  @media ${tablet} {
    display: none;
  }

  .plugins__plugin {
    width: 30%;
    margin-right: 12px;
  }

  .plugins__text-container {
    display: flex;
    margin-left: 6px;
    width: 70%;
  }

  .plugins__separator {
    display: block;
    height: 10px;
    margin: 4px 8px 0 0;
    z-index: 1;
    border-right: ${(props) => props.theme.connectedClouds.borderRight};
  }
`;

StyledHeader.defaultProps = { theme: Base };

const StyledRow = styled(Row)`
  .row-main-container-wrapper {
    margin-right: 40px;
  }
`;

const PluginList = ({ t, plugins, onActivate, onDelete, theme }) => {
  const onActivateAction = React.useCallback(
    (e) => {
      const { dataset } = (e.originalEvent || e).currentTarget;

      activatePlugin(dataset.id, dataset.status);
      onActivate(dataset.id, dataset.status);
    },
    [onActivate]
  );

  const onDeleteAction = React.useCallback(
    (e) => {
      const { dataset } = (e.originalEvent || e).currentTarget;

      deletePlugin(dataset.id);
      onDelete(dataset.id);
    },
    [onDelete]
  );

  const getContextOptions = React.useCallback(
    (plugin, index) => {
      const activateItem = plugin.isActive
        ? {
            key: `${index}_disable`,
            "data-id": plugin.id,
            "data-status": !plugin.isActive,
            label: t("PeopleTranslations:DisableUserButton"),
            onClick: onActivateAction,
          }
        : {
            key: `${index}_activate`,
            "data-id": plugin.id,
            "data-status": !plugin.isActive,
            label: t("Common:Activate"),
            onClick: onActivateAction,
          };

      const deleteItem = {
        key: "delete",
        "data-id": plugin.id,
        label: t("Common:Delete"),
        onClick: onDeleteAction,
      };

      return [activateItem, deleteItem];
    },
    [onActivateAction, onDeleteAction]
  );

  return (
    <>
      <StyledHeader>
        <Text
          className="plugins__plugin"
          fontSize="12px"
          fontWeight={600}
          color={theme.connectedClouds.color}
          noSelect
        >
          {t("Common:Plugin")}
        </Text>
        <div></div>

        <div className="plugins__text-container">
          <div className="plugins__separator" />
          <Text
            className="plugins__status"
            fontSize="12px"
            fontWeight={600}
            color={theme.connectedClouds.color}
            noSelect
          >
            {t("People:UserStatus")}
          </Text>
        </div>
      </StyledHeader>
      <RowContainer useReactWindow={false}>
        {plugins
          ? plugins.map((plugin, index) => {
              const name = plugin.getPluginName();
              const version = plugin.getPluginVersion();

              return (
                <StyledRow
                  key={plugin.id}
                  contextOptions={getContextOptions(plugin, index)}
                >
                  <RowContent>
                    <Text
                      as="div"
                      type="page"
                      fontSize="13px"
                      fontWeight={600}
                      title={plugin.name}
                      noSelect
                      containerWidth="30%"
                    >
                      {name} {version}
                    </Text>
                    <div></div>

                    <Text
                      as="div"
                      type="page"
                      fontSize="13px"
                      fontWeight={600}
                      title={plugin.isActive ? "Active" : "Disabled"}
                      noSelect
                      containerWidth="70%"
                    >
                      {plugin.isActive ? "Active" : "Disabled"}
                    </Text>
                  </RowContent>
                </StyledRow>
              );
            })
          : "Loading"}
      </RowContainer>
    </>
  );
};

export default React.memo(PluginList);

import React from "react";
import Text from "@docspace/components/text";
import Avatar from "@docspace/components/avatar";

import { StyledRow, StyledComboBox } from "./StyledInvitePanel";

const Items = ({ t, items, onSelectItemAccess }) => {
  const getAccesses = (id) => {
    return [
      {
        key: "roomManager",
        label: "Room manager",
        id,
      },
      {
        key: "editor",
        label: "Editor",
        id,
      },
      {
        key: "formFiller",
        label: "Form filler",
        id,
      },
      {
        key: "reviewer",
        label: "Reviewer",
        id,
      },
      {
        key: "commentator",
        label: "Commentator",
        id,
      },
      {
        key: "viewer",
        label: "Viewer",
        id,
      },
      {
        key: "sep",
        isSeparator: true,
        id,
      },
      {
        key: "delete",
        label: "Delete",
        id,
      },
    ];
  };

  return items.map((item) => {
    const { avatarSmall, displayName, email, id } = item;

    const name = !!avatarSmall ? displayName : email;
    const source = !!avatarSmall ? avatarSmall : "/static/images/@.react.svg";
    const options = getAccesses(id);

    return (
      <StyledRow key={id}>
        <Avatar size="min" role="user" source={source} />
        <Text>{name}</Text>
        <StyledComboBox
          onSelect={onSelectItemAccess}
          noBorder
          options={options}
          size="content"
          scaled={false}
          manualWidth="fit-content"
          selectedOption={options[5]}
          showDisabledItems
        />
      </StyledRow>
    );
  });
};
export default Items;

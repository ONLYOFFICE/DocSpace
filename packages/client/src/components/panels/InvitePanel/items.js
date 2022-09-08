import React, { useState } from "react";
import { inject, observer } from "mobx-react";

import Text from "@docspace/components/text";
import Avatar from "@docspace/components/avatar";

import { parseAddresses } from "@docspace/components/utils/email";
import { getAccessOptions } from "./accesses";

import {
  StyledRow,
  StyledComboBox,
  StyledEditInput,
  StyledEditButton,
  StyledCheckIcon,
  StyledCrossIcon,
  StyledHelpButton,
  StyledDeleteIcon,
} from "./StyledInvitePanel";

const Item = ({ t, item, setInviteItems, inviteItems, changeInviteItem }) => {
  const { avatarSmall, displayName, email, id, errors, access } = item;

  const name = !!avatarSmall ? displayName : email;
  const source = !!avatarSmall ? avatarSmall : "/static/images/@.react.svg";

  const [edit, setEdit] = useState(false);
  const [inputValue, setInputValue] = useState(name);
  const [parseErrors, setParseErrors] = useState(errors);

  const accesses = getAccessOptions(t, 5, true);

  const defaultAccess = accesses.find((option) => option.access === access);

  const onEdit = (e) => {
    if (e.detail === 2) {
      setEdit(true);
    }
  };

  const cancelEdit = (e) => {
    setInputValue(name);
    setEdit(false);
  };

  const saveEdit = (e) => {
    const value = inputValue === "" ? name : inputValue;

    setEdit(false);
    validateValue(value);
  };

  const validateValue = (value) => {
    const email = parseAddresses(value);
    const parseErrors = email[0].parseErrors;
    const errors = !!parseErrors.length ? parseErrors : [];

    setParseErrors(errors);
    changeInviteItem({ id, email: value, errors });
  };

  const changeValue = (e) => {
    const value = e.target.value.trim();

    setInputValue(value);
  };

  const hasError = parseErrors && !!parseErrors.length;

  const removeItem = () => {
    const newItems = inviteItems.filter((item) => item.id !== id);

    setInviteItems(newItems);
  };

  const selectItemAccess = (selected) => {
    if (selected.key === "remove") return removeItem();

    changeInviteItem({ id, access: selected.access });
  };

  const textProps = !!avatarSmall ? {} : { onClick: onEdit };

  const displayBody = (
    <>
      <Text {...textProps} noSelect>
        {inputValue}
      </Text>
      {hasError ? (
        <>
          <StyledHelpButton
            iconName="/static/images/info.edit.react.svg"
            displayType="auto"
            offsetRight={0}
            tooltipContent={t("EmailErrorMessage")}
            size={16}
            color="#F21C0E"
          />
          <StyledDeleteIcon size="medium" onClick={removeItem} />
        </>
      ) : (
        <StyledComboBox
          onSelect={selectItemAccess}
          noBorder
          options={accesses}
          size="content"
          scaled={false}
          manualWidth="fit-content"
          selectedOption={defaultAccess}
          showDisabledItems
        />
      )}
    </>
  );

  const okIcon = <StyledCheckIcon size="scale" />;
  const cancelIcon = <StyledCrossIcon size="scale" />;

  const editBody = (
    <>
      <StyledEditInput value={inputValue} onChange={changeValue} />
      <StyledEditButton icon={okIcon} onClick={saveEdit} />
      <StyledEditButton icon={cancelIcon} onClick={cancelEdit} />
    </>
  );

  return (
    <>
      <Avatar size="min" role="user" source={source} />
      {edit ? editBody : displayBody}
    </>
  );
};

const Items = ({ t, setInviteItems, inviteItems, changeInviteItem }) => {
  return inviteItems.map((item) => (
    <StyledRow key={item.id}>
      <Item
        t={t}
        item={item}
        setInviteItems={setInviteItems}
        changeInviteItem={changeInviteItem}
        inviteItems={inviteItems}
      />
    </StyledRow>
  ));
};

export default inject(({ dialogsStore }) => {
  const { setInviteItems, inviteItems, changeInviteItem } = dialogsStore;

  return {
    setInviteItems,
    inviteItems,
    changeInviteItem,
  };
})(observer(Items));

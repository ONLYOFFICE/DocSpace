import React, { useState, useEffect } from "react";

import Text from "@docspace/components/text";
import Avatar from "@docspace/components/avatar";

import { parseAddresses } from "@docspace/components/utils/email";
import { getAccessOptions } from "../utils";

import {
  StyledComboBox,
  StyledEditInput,
  StyledEditButton,
  StyledCheckIcon,
  StyledCrossIcon,
  StyledHelpButton,
  StyledDeleteIcon,
} from "../StyledInvitePanel";

const Item = ({
  t,
  item,
  setInviteItems,
  inviteItems,
  changeInviteItem,
  setHasErrors,
  roomType,
  isOwner,
}) => {
  const { avatar, displayName, email, id, errors, access } = item;

  const name = !!avatar ? (displayName !== "" ? displayName : email) : email;
  const source = !!avatar ? avatar : "/static/images/@.react.svg";

  const [edit, setEdit] = useState(false);
  const [inputValue, setInputValue] = useState(name);
  const [parseErrors, setParseErrors] = useState(errors);

  const accesses = getAccessOptions(t, roomType, true, false, isOwner);

  const defaultAccess = accesses.find((option) => option.access === +access);

  const errorsInList = () => {
    const hasErrors = inviteItems.some((item) => !!item.errors?.length);
    setHasErrors(hasErrors);
  };

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

  const onKeyPress = (e) => {
    if (edit) {
      if (e.key === "Enter") {
        saveEdit();
      }
    }
  };

  useEffect(() => {
    document.addEventListener("keyup", onKeyPress);
    return () => document.removeEventListener("keyup", onKeyPress);
  });

  const validateValue = (value) => {
    const email = parseAddresses(value);
    const parseErrors = email[0].parseErrors;
    const errors = !!parseErrors.length ? parseErrors : [];

    setParseErrors(errors);
    changeInviteItem({ id, email: value, errors }).then(() => errorsInList());
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

  const textProps = !!avatar ? {} : { onClick: onEdit };

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
          modernView
          directionX="right"
          directionY="bottom"
          isDefaultMode={false}
          fixedDirection={true}
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

export default Item;

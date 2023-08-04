import InfoEditReactSvgUrl from "PUBLIC_DIR/images/info.edit.react.svg?url";
import AtReactSvgUrl from "PUBLIC_DIR/images/@.react.svg?url";
import React, { useState, useEffect } from "react";
import Avatar from "@docspace/components/avatar";
import Text from "@docspace/components/text";

import { parseAddresses } from "@docspace/components/utils/email";
import { getAccessOptions } from "../utils";

import {
  StyledEditInput,
  StyledEditButton,
  StyledCheckIcon,
  StyledCrossIcon,
  StyledHelpButton,
  StyledDeleteIcon,
} from "../StyledInvitePanel";
import { filterUserRoleOptions } from "SRC_DIR/helpers/utils";
import AccessSelector from "./AccessSelector";

const Item = ({
  t,
  item,
  setInviteItems,
  inviteItems,
  changeInviteItem,
  setHasErrors,
  roomType,
  isOwner,
  inputsRef,
  setIsOpenItemAccess,
}) => {
  const { avatar, displayName, email, id, errors, access } = item;

  const name = !!avatar ? (displayName !== "" ? displayName : email) : email;
  const source = !!avatar ? avatar : AtReactSvgUrl;

  const [edit, setEdit] = useState(false);
  const [inputValue, setInputValue] = useState(name);
  const [parseErrors, setParseErrors] = useState(errors);

  const accesses = getAccessOptions(t, roomType, true, true, isOwner);

  const filteredAccesses = filterUserRoleOptions(accesses, item, true);

  const defaultAccess = filteredAccesses.find(
    (option) => option.access === +access
  );

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
      <Text {...textProps} truncate noSelect>
        {inputValue}
      </Text>
      {hasError ? (
        <>
          <StyledHelpButton
            iconName={InfoEditReactSvgUrl}
            displayType="auto"
            offsetRight={0}
            tooltipContent={t("EmailErrorMessage")}
            size={16}
            color="#F21C0E"
          />
          <StyledDeleteIcon
            className="delete-icon"
            size="medium"
            onClick={removeItem}
          />
        </>
      ) : (
        <AccessSelector
          className="user-access"
          t={t}
          roomType={roomType}
          defaultAccess={defaultAccess?.access}
          onSelectAccess={selectItemAccess}
          containerRef={inputsRef}
          isOwner={isOwner}
          withRemove={true}
          filteredAccesses={filteredAccesses}
          setIsOpenItemAccess={setIsOpenItemAccess}
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

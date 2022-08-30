import React, { useState, useCallback } from "react";
import debounce from "lodash.debounce";

import Text from "@docspace/components/text";
import Avatar from "@docspace/components/avatar";

import { parseAddresses } from "@docspace/components/utils/email";

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

const Item = ({ item, onSelectItemAccess }) => {
  const { avatar, avatarSmall, displayName, email, id, errors } = item;

  const userAvatar = avatar || avatarSmall;
  const name = !!userAvatar ? displayName : email;
  const source = !!userAvatar ? avatarSmall : "/static/images/@.react.svg";

  const [edit, setEdit] = useState(false);
  const [inputValue, setInputValue] = useState(name);
  const [parseErrors, setParseErrors] = useState(errors);

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

  const onEdit = (e) => {
    if (e.detail === 2) {
      setEdit(true);
    }
  };

  const onCancelEdit = (e) => {
    setInputValue(name);
    setEdit(false);
  };

  const onSaveEdit = (e) => {
    if (inputValue === "") {
      setInputValue(name);
    }

    setEdit(false);

    debouncedValidate(inputValue);

    console.log(parseErrors);
  };

  const validateValue = (value) => {
    const email = parseAddresses(value);
    const errors = email[0].parseErrors;

    if (!!errors.length) {
      setParseErrors(errors);
    } else {
      setParseErrors([]);
    }
  };

  const debouncedValidate = useCallback(
    debounce((value) => validateValue(value), 500),
    []
  );

  const onChangeValue = (e) => {
    const value = e.target.value.trim();
    setInputValue(value);

    debouncedValidate(value);
  };

  const options = getAccesses(id);

  const hasError = !!parseErrors.length;

  const tooltipBody = parseErrors.map((error) => (
    <div key={error.key}>{error.message}</div>
  ));

  const removeItem = (e) => {
    const id = e.target.dataset.id;

    onSelectItemAccess({
      key: "delete",
      id,
    });
  };

  const displayBody = (
    <>
      <Text onClick={onEdit}>{inputValue}</Text>
      {hasError ? (
        <>
          <StyledHelpButton
            iconName="/static/images/info.edit.react.svg"
            displayType="auto"
            offsetRight={0}
            tooltipContent={tooltipBody}
            size={16}
            color="#F21C0E"
          />
          <StyledDeleteIcon size="medium" onClick={removeItem} data-id={id} />
        </>
      ) : (
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
      )}
    </>
  );

  const okIcon = <StyledCheckIcon size="scale" />;
  const cancelIcon = <StyledCrossIcon size="scale" />;

  const editBody = (
    <>
      <StyledEditInput hasError value={inputValue} onChange={onChangeValue} />
      <StyledEditButton icon={okIcon} onClick={onSaveEdit} />
      <StyledEditButton icon={cancelIcon} onClick={onCancelEdit} />
    </>
  );

  return (
    <>
      <Avatar size="min" role="user" source={source} />
      {edit ? editBody : displayBody}
    </>
  );
};

const Items = ({ t, items, onSelectItemAccess }) => {
  return items.map((item) => (
    <StyledRow key={item.id}>
      <Item item={item} onSelectItemAccess={onSelectItemAccess} />
    </StyledRow>
  ));
};
export default Items;

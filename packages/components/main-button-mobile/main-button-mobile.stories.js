import React from "react";
import MainButtonMobile from ".";
import { useEffect, useReducer, useState } from "react";
import MobileActionsFolderReactSvgUrl from "PUBLIC_DIR/images/mobile.actions.folder.react.svg?url";
import MobileActionsRemoveReactSvgUrl from "PUBLIC_DIR/images/mobile.actions.remove.react.svg?url";
import MobileStartReactSvgUrl from "PUBLIC_DIR/images/mobile.star.react.svg?url";

import MobileMainButtonDocs from "./main-button-mobile-docs.mdx";

export default {
  title: "Components/MainButtonMobile",
  component: MainButtonMobile,
  tags: ["autodocs"],
  parameters: {
    docs: {
      page: MobileMainButtonDocs,
    },
  },
};

const actionOptions = [
  {
    key: "1",
    label: "New document",
    icon: MobileActionsFolderReactSvgUrl,
  },
  {
    key: "2",
    label: "New presentation",
    icon: MobileActionsFolderReactSvgUrl,
  },
  {
    key: "3",
    label: "New spreadsheet",
    icon: MobileActionsFolderReactSvgUrl,
  },
  {
    key: "4",
    label: "New folder",
    icon: MobileActionsFolderReactSvgUrl,
  },
];

const buttonOptions = [
  {
    key: "1",
    label: "Import point",
    icon: MobileStartReactSvgUrl,
    onClick: () => setIsOpenButton(false),
  },
  {
    key: "2",
    label: "Import point",
    icon: MobileStartReactSvgUrl,
    onClick: () => setIsOpenButton(false),
  },
  {
    key: "3",
    label: "Import point",
    isSeparator: true,
  },
  {
    key: "4",
    label: "Import point",
    icon: MobileStartReactSvgUrl,
    onClick: () => setIsOpenButton(false),
  },
];

const Template = ({ ...args }) => {
  const maxUploads = 10;
  const maxOperations = 7;

  const [isOpenUploads, setIsOpenUploads] = useState(false);
  const [isOpenOperations, setIsOpenOperations] = useState(false);

  const [isOpenButton, setIsOpenButton] = useState(false);
  const [opened, setOpened] = useState(null);

  const [isUploading, setIsUploading] = useState(false);

  const [initialState, setInitialState] = useState({
    uploads: 0,
    operations: 0,
  });
  const onUploadClick = () => {
    setInitialState({ uploads: 0, operations: 0 });
    setIsUploading(true);
    setIsOpenUploads(true);
    setIsOpenOperations(true);
    setIsOpenButton(true);
    //  setOpened(false);
  };

  function reducer(state, action) {
    switch (action.type) {
      case "start":
        if (
          state.uploads === maxUploads &&
          state.operations === maxOperations
        ) {
          setIsUploading(false);
          return {
            ...state,
            uploads: state.uploads,
            operations: state.operations,
          };
        }
        return {
          ...state,
          uploads:
            state.uploads !== maxUploads ? state.uploads + 1 : state.uploads,
          operations:
            state.operations !== maxOperations
              ? state.operations + 1
              : state.operations,
        };
      default:
        return state;
    }
  }

  const [state, dispatch] = useReducer(reducer, initialState);

  useEffect(() => {
    setOpened(null);
    if (isUploading) {
      const id = setInterval(() => {
        dispatch({ type: "start" });
      }, 1000);

      return () => clearInterval(id);
    }
  }, [dispatch, isUploading]);

  const uploadPercent = (state.uploads / maxUploads) * 100;
  const operationPercent = (state.operations / maxOperations) * 100;

  const progressOptions = [
    {
      key: "1",
      label: "Uploads",
      icon: MobileActionsRemoveReactSvgUrl,
      percent: uploadPercent,
      status: `${state.uploads}/${maxUploads}`,
      open: isOpenUploads,
      onCancel: () => setIsOpenUploads(false),
    },
    {
      key: "2",
      label: "Other operations",
      icon: MobileActionsRemoveReactSvgUrl,
      percent: operationPercent,
      status: `3 files not loaded`,
      open: isOpenOperations,
      onCancel: () => setIsOpenOperations(false),
      error: true,
    },
  ];

  return (
    <div style={{ width: "600px", height: "500px" }}>
      <MainButtonMobile
        {...args}
        style={{ position: "absolute", bottom: "26px", right: "44px" }}
        actionOptions={actionOptions}
        dropdownStyle={{ position: "absolute", right: "60px", bottom: "25px" }}
        progressOptions={progressOptions}
        buttonOptions={buttonOptions}
        onUploadClick={onUploadClick}
        withButton={true}
        isOpenButton={isOpenButton}
        isDefaultMode={false}
        title="Upload"
        percent={uploadPercent}
        opened={opened}
      />
    </div>
  );
};

export const Default = Template.bind({});
Default.args = {
  title: "Upload",
  percent: 0,
  opened: null,
};

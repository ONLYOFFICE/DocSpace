import React from "react";
import MainButtonMobile from ".";
import { useEffect, useReducer, useState } from "react";

export default {
  title: "Components/MainButtonMobile",
  component: MainButtonMobile,
  parameters: {
    docs: { description: { component: "Components/MainButtonMobile" } },
  },
};

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

  const actionOptions = [
    {
      key: "1",
      label: "New document",
      icon: "static/images/mobile.actions.document.react.svg",
    },
    {
      key: "2",
      label: "New presentation",
      icon: "static/images/mobile.actions.presentation.react.svg",
    },
    {
      key: "3",
      label: "New spreadsheet",
      icon: "static/images/mobile.actions.spreadsheet.react.svg",
    },
    {
      key: "4",
      label: "New folder",
      icon: "static/images/mobile.actions.folder.react.svg",
    },
  ];

  const progressOptions = [
    {
      key: "1",
      label: "Uploads",
      icon: "/static/images/mobile.actions.remove.react.svg",
      percent: uploadPercent,
      status: `${state.uploads}/${maxUploads}`,
      open: isOpenUploads,
      onCancel: () => setIsOpenUploads(false),
    },
    {
      key: "2",
      label: "Other operations",
      icon: "/static/images/mobile.actions.remove.react.svg",
      percent: operationPercent,
      status: `3 files not loaded`,
      open: isOpenOperations,
      onCancel: () => setIsOpenOperations(false),
      error: true,
    },
  ];

  const buttonOptions = [
    {
      key: "1",
      label: "Import point",
      icon: "static/images/mobile.star.react.svg",
      onClick: () => setIsOpenButton(false),
    },
    {
      key: "2",
      label: "Import point",
      icon: "static/images/mobile.star.react.svg",
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
      icon: "static/images/mobile.star.react.svg",
      onClick: () => setIsOpenButton(false),
    },
  ];

  return (
    <MainButtonMobile
      {...args}
      style={{ position: "absolute", top: "87%", left: "87%" }}
      actionOptions={actionOptions}
      progressOptions={progressOptions}
      buttonOptions={buttonOptions}
      onUploadClick={onUploadClick}
      withButton={true}
      isOpenButton={isOpenButton}
      title="Upload"
      percent={uploadPercent}
      opened={opened}
    />
  );
};

export const Default = Template.bind({});
Default.args = {
  title: "Upload",
  percent: 0,
  opened: null,
};

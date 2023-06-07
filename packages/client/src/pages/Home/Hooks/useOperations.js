import React from "react";
import { Trans } from "react-i18next";

import toastr from "@docspace/components/toast/toastr";

const useOperations = ({
  t,
  setUploadPanelVisible,
  primaryProgressDataVisible,
  uploaded,
  converted,
  clearPrimaryProgressData,
  isProgressFinished,
  refreshFiles,
  itemsSelectionTitle,
  secondaryProgressDataStoreIcon,
  itemsSelectionLength,
  isAccountsPage,
  isSettingsPage,
  setItemsSelectionTitle,
}) => {
  const prevProps = React.useRef({
    isProgressFinished: isProgressFinished,
  });

  React.useEffect(() => {
    if (
      isProgressFinished &&
      itemsSelectionTitle &&
      isProgressFinished !== prevProps.current.isProgressFinished
    ) {
      showOperationToast(
        secondaryProgressDataStoreIcon,
        itemsSelectionLength,
        itemsSelectionTitle
      );
      setItemsSelectionTitle(null);
    }
  }, [
    isProgressFinished,

    itemsSelectionTitle,
    showOperationToast,
    setItemsSelectionTitle,
  ]);

  React.useEffect(() => {
    prevProps.current.isProgressFinished = isProgressFinished;
  }, [isProgressFinished]);

  const showUploadPanel = () => {
    setUploadPanelVisible(true);

    if (primaryProgressDataVisible && uploaded && converted)
      clearPrimaryProgressData();
  };

  const showOperationToast = React.useCallback(
    (type, qty, title) => {
      switch (type) {
        case "move":
          if (qty > 1) {
            return (
              toastr.success(
                <Trans t={t} i18nKey="MoveItems" ns="Files">
                  {{ qty }} elements has been moved
                </Trans>
              ),
              refreshFiles()
            );
          }
          return (
            toastr.success(
              <Trans t={t} i18nKey="MoveItem" ns="Files">
                {{ title }} moved
              </Trans>
            ),
            refreshFiles()
          );
        case "duplicate":
          if (qty > 1) {
            return (
              toastr.success(
                <Trans t={t} i18nKey="CopyItems" ns="Files">
                  {{ qty }} elements copied
                </Trans>
              ),
              refreshFiles()
            );
          }
          return (
            toastr.success(
              <Trans t={t} i18nKey="CopyItem" ns="Files">
                {{ title }} copied
              </Trans>
            ),
            refreshFiles()
          );

        default:
          break;
      }
    },
    [t]
  );

  return { showUploadPanel };
};

export default useOperations;

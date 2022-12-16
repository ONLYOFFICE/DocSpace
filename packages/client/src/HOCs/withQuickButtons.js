import React from "react";
import { inject, observer } from "mobx-react";
import toastr from "@docspace/components/toast/toastr";
import QuickButtons from "../components/QuickButtons";

export default function withQuickButtons(WrappedComponent) {
  class WithQuickButtons extends React.Component {
    constructor(props) {
      super(props);

      this.state = {
        isLoading: false,
        isCanWebEdit: props.item.viewAccessability?.WebEdit,
      };
    }

    onClickLock = () => {
      const { item, lockFileAction, isAdmin, t } = this.props;
      const { locked, id, access } = item;

      if ((isAdmin || access === 0) && !this.state.isLoading) {
        this.setState({ isLoading: true });
        return lockFileAction(id, !locked)
          .then(() =>
            locked
              ? toastr.success(t("Translations:FileUnlocked"))
              : toastr.success(t("Translations:FileLocked"))
          )
          .catch((err) => toastr.error(err))
          .finally(() => this.setState({ isLoading: false }));
      }
      return;
    };

    onClickFavorite = (showFavorite) => {
      const { t, item, setFavoriteAction } = this.props;

      if (showFavorite) {
        setFavoriteAction("remove", item.id)
          .then(() => toastr.success(t("RemovedFromFavorites")))
          .catch((err) => toastr.error(err));
        return;
      }

      setFavoriteAction("mark", item.id)
        .then(() => toastr.success(t("MarkedAsFavorite")))
        .catch((err) => toastr.error(err));
    };

    render() {
      const { isLoading, isCanWebEdit } = this.state;

      const { t, theme, item, isAdmin, sectionWidth, viewAs } = this.props;

      const quickButtonsComponent = (
        <QuickButtons
          t={t}
          theme={theme}
          item={item}
          sectionWidth={sectionWidth}
          isAdmin={isAdmin}
          viewAs={viewAs}
          isDisabled={isLoading}
          isCanWebEdit={isCanWebEdit}
          onClickLock={this.onClickLock}
          onClickFavorite={this.onClickFavorite}
        />
      );

      return (
        <WrappedComponent
          quickButtonsComponent={quickButtonsComponent}
          {...this.props}
        />
      );
    }
  }

  return inject(
    ({
      auth,
      filesActionsStore,
      dialogsStore,

      treeFoldersStore,
    }) => {
      const {
        lockFileAction,
        setFavoriteAction,
        onSelectItem,
      } = filesActionsStore;
      const { isPersonalRoom } = treeFoldersStore;
      const { setSharingPanelVisible } = dialogsStore;

      return {
        theme: auth.settingsStore.theme,
        isAdmin: auth.isAdmin,
        lockFileAction,
        setFavoriteAction,
        onSelectItem,
        setSharingPanelVisible,

        isPersonalRoom,
      };
    }
  )(observer(WithQuickButtons));
}

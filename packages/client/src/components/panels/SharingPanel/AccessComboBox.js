import React from "react";
import ComboBox from "@docspace/components/combobox";
import { ShareAccessRights } from "@docspace/common/constants";
import DropDownItem from "@docspace/components/drop-down-item";
import AccessRightSelect from "@docspace/components/access-right-select";
import { getAccessIcon } from "../../../helpers/files-helpers";
import { ReactSVG } from "react-svg";
import Backdrop from "@docspace/components/backdrop";

const {
  FullAccess,
  CustomFilter,
  Review,
  FormFilling,
  Comment,
  ReadOnly,
  DenyAccess,
} = ShareAccessRights;

const AccessComboBox = (props) => {
  const {
    access,
    accessOptions,
    directionY,
    directionX,
    isDisabled,
    itemId,
    onAccessChange,
    t,
    theme,
    disableLink,
    fixedDirection,
    canDelete,
    onRemoveUserClick,
    isExternalLink,
    isDefaultMode,
    isEmbedded,
    isPersonal,
  } = props;

  const [isLoading, setIsLoading] = React.useState(true);
  const [availableOptions, setAvailableOptions] = React.useState([]);
  const [selectedOption, setSelectedOption] = React.useState(null);

  const ref = React.useRef(null);

  const onSelect = React.useCallback(
    (e) => {
      const access = +e.target.dataset.access;

      if (access) {
        const item = availableOptions.find((option) => {
          return option.dataAccess === access;
        });

        setSelectedOption(item);

        onAccessChange && onAccessChange(e);
      } else {
        onRemoveUserClick && onRemoveUserClick(e);
      }
    },
    [availableOptions, onAccessChange, onRemoveUserClick]
  );

  React.useEffect(() => {
    const accessRights = disableLink ? ReadOnly : access;

    const newAvailableOptions = [];

    if (accessOptions.includes("FullAccess")) {
      const accessItem = {
        key: FullAccess,
        title: t("Common:FullAccess"),
        label: t("Common:FullAccess"),
        icon: "/static/images/access.edit.react.svg",
        itemId: itemId,
        dataAccess: FullAccess,
      };

      newAvailableOptions.push(accessItem);

      if (accessRights === FullAccess) {
        setSelectedOption(accessItem);
      }
    }

    if (accessOptions.includes("FilterEditing")) {
      const filterItem = {
        key: CustomFilter,
        title: t("CustomFilter"),
        label: t("CustomFilter"),
        icon: "/static/images/custom.filter.react.svg",
        itemId: itemId,
        dataAccess: CustomFilter,
      };

      newAvailableOptions.push(filterItem);

      if (accessRights === CustomFilter) {
        setSelectedOption(filterItem);
      }
    }

    if (accessOptions.includes("Review")) {
      const reviewItem = {
        key: Review,
        title: t("Common:Review"),
        label: t("Common:Review"),
        icon: "/static/images/access.review.react.svg",
        itemId: itemId,
        dataAccess: Review,
      };

      newAvailableOptions.push(reviewItem);

      if (accessRights === Review) {
        setSelectedOption(reviewItem);
      }
    }

    if (accessOptions.includes("FormFilling")) {
      const formItem = {
        key: FormFilling,
        title: t("FormFilling"),
        label: t("FormFilling"),
        icon: "/static/images/access.form.react.svg",
        itemId: itemId,
        dataAccess: FormFilling,
      };

      newAvailableOptions.push(formItem);

      if (accessRights === FormFilling) {
        setSelectedOption(formItem);
      }
    }

    if (accessOptions.includes("Comment")) {
      const commentItem = {
        key: Comment,
        title: t("Comment"),
        label: t("Comment"),
        icon: "/static/images/access.comment.react.svg",
        itemId: itemId,
        dataAccess: Comment,
      };

      newAvailableOptions.push(commentItem);

      if (accessRights === Comment) {
        setSelectedOption(commentItem);
      }
    }

    if (accessOptions.includes("ReadOnly")) {
      const readItem = {
        key: ReadOnly,
        title: t("ReadOnly"),
        label: t("ReadOnly"),
        icon: "/static/images/eye.react.svg",
        itemId: itemId,
        dataAccess: ReadOnly,
      };

      newAvailableOptions.push(readItem);

      if (accessRights === ReadOnly) {
        setSelectedOption(readItem);
      }
    }

    if (accessOptions.includes("DenyAccess")) {
      const denyItem = {
        key: DenyAccess,
        title: t("DenyAccess"),
        label: t("DenyAccess"),
        icon: "/static/images/access.none.react.svg",
        itemId: itemId,
        dataAccess: DenyAccess,
      };

      newAvailableOptions.push(denyItem);

      if (accessRights === DenyAccess) {
        setSelectedOption(denyItem);
      }
    }

    if (canDelete) {
      newAvailableOptions.push({ key: "separator", isSeparator: true });
      newAvailableOptions.push({
        key: "delete",
        title: t("Common:Delete"),
        label: t("Common:Delete"),
        icon: "/static/images/delete.react.svg",
        dataFor: itemId,
        onClick: onRemoveUserClick,
      });
    }

    setAvailableOptions(newAvailableOptions);
    if (newAvailableOptions.length > 0) {
      setIsLoading(false);
    }
  }, [
    access,
    disableLink,
    accessOptions,
    onRemoveUserClick,
    itemId,
    canDelete,
  ]);

  const renderAdvancedOption = React.useCallback(() => {
    return (
      <>
        {availableOptions?.map((option) => (
          <DropDownItem
            key={option.key}
            label={option.label}
            icon={option.icon}
            data-id={option.itemId}
            data-access={option.dataAccess}
            data-for={option.dataFor}
            onClick={onSelect}
            isSeparator={option.isSeparator}
          />
        ))}
      </>
    );
  }, [availableOptions, onSelect]);

  const advancedOptions = renderAdvancedOption();

  if (isLoading) {
    return <></>;
  }

  return isExternalLink ? (
    <AccessRightSelect
      options={[]}
      selectedOption={selectedOption}
      advancedOptions={advancedOptions}
      isExternalLink={isExternalLink}
      isPersonal={isPersonal}
    />
  ) : (
    <ComboBox
      theme={theme}
      advancedOptions={advancedOptions}
      options={[]}
      selectedOption={{}}
      size="content"
      className={`panel_combo-box ${isEmbedded ? "embedded_combo-box" : ""}`}
      scaled={false}
      directionX={directionX}
      directionY={directionY}
      disableIconClick={false}
      isDisabled={isDisabled}
      isDefaultMode={isDefaultMode}
      ref={ref}
      forwardRef={ref}
      fixedDirection={fixedDirection}
    >
      {selectedOption?.icon && (
        <ReactSVG
          src={selectedOption?.icon}
          className="sharing-access-combo-box-icon"
        />
      )}
    </ComboBox>
  );
};

export default React.memo(AccessComboBox);

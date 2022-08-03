import React from "react";
import PropTypes from "prop-types";
import { I18nextProvider, withTranslation } from "react-i18next";
import i18n from "./i18n";
import AdvancedSelector from "@docspace/common/components/AdvancedSelector";
import { getGroupList } from "@docspace/common/api/groups";

class GroupSelector extends React.Component {
  constructor(props) {
    super(props);

    const { isOpen } = props;
    this.state = this.getDefaultState(isOpen, []);
  }

  componentDidMount() {
    getGroupList(this.props.useFake)
      .then((groups) => this.setState({ options: this.convertGroups(groups) }))
      .catch((error) => console.log(error));
  }

  componentDidUpdate(prevProps) {
    if (this.props.isOpen !== prevProps.isOpen)
      this.setState({ isOpen: this.props.isOpen });
  }

  convertGroups = (groups) => {
    return groups
      ? groups.map((g) => {
          return {
            key: g.id,
            label: g.name,
            total: 0,
            selectedCount: 0,
          };
        })
      : [];
  };

  loadNextPage = ({ startIndex, searchValue }) => {
    console.log(
      `loadNextPage(startIndex=${startIndex}, searchValue="${searchValue}")`
    );

    this.setState({ isNextPageLoading: true }, () => {
      getGroupList(this.props.useFake, searchValue)
        .then((groups) => {
          const newOptions = this.convertGroups(groups);

          this.setState({
            hasNextPage: false,
            isNextPageLoading: false,
            options: newOptions,
          });
        })
        .catch((error) => console.log(error));
    });
  };

  getDefaultState = (isOpen, options) => {
    return {
      isOpen: isOpen,
      options,
      hasNextPage: true,
      isNextPageLoading: false,
    };
  };

  onSearchChanged = (value) => {
    //action("onSearchChanged")(value);
    console.log("Search group", value);
    this.setState({ options: [], hasNextPage: true });
  };

  render() {
    const { isOpen, options, hasNextPage, isNextPageLoading } = this.state;

    const {
      id,
      className,
      style,
      isMultiSelect,
      isDisabled,
      onSelect,
      onCancel,
      t,
      searchPlaceHolderLabel,
      headerLabel,
      onArrowClick,
      withoutAside,
      embeddedComponent,
      showCounter,
      smallSectionWidth,
      theme,
      selectedOptions,
    } = this.props;

    const groups = [
      {
        key: "all",
        id: "all",
        label: t("AllGroups"),
        total: options.length,
        selectedCount: 0,
      },
    ];

    return (
      <AdvancedSelector
        theme={theme}
        id={id}
        className={className}
        style={style}
        options={options}
        groups={groups}
        hasNextPage={hasNextPage}
        smallSectionWidth={smallSectionWidth}
        isNextPageLoading={isNextPageLoading}
        loadNextPage={this.loadNextPage}
        size={"compact"}
        selectedOptions={selectedOptions}
        isOpen={isOpen}
        isMultiSelect={isMultiSelect}
        isDefaultDisplayDropDown={false}
        isDisabled={isDisabled}
        searchPlaceHolderLabel={
          searchPlaceHolderLabel || t("SearchPlaceholder")
        }
        selectButtonLabel={t("AddDepartmentsButtonLabel")}
        selectAllLabel={t("Common:SelectAll")}
        emptySearchOptionsLabel={t("EmptySearchOptionsLabel")}
        emptyOptionsLabel={t("EmptyOptionsLabel")}
        loadingLabel={`${t("Common:LoadingProcessing")} ${t(
          "Common:LoadingDescription"
        )}`}
        onSelect={onSelect}
        onSearchChanged={this.onSearchChanged}
        onCancel={onCancel}
        withoutAside={withoutAside}
        embeddedComponent={embeddedComponent}
        showCounter={showCounter}
        headerLabel={headerLabel ? headerLabel : t("AddDepartmentsButtonLabel")}
        onArrowClick={onArrowClick}
      />
    );
  }
}

GroupSelector.propTypes = {
  className: PropTypes.oneOfType([PropTypes.string, PropTypes.array]),
  id: PropTypes.string,
  isDisabled: PropTypes.bool,
  isMultiSelect: PropTypes.bool,
  isOpen: PropTypes.bool,
  language: PropTypes.string,
  onCancel: PropTypes.func,
  onSelect: PropTypes.func,
  searchPlaceHolderLabel: PropTypes.string,
  style: PropTypes.object,
  t: PropTypes.func,
  useFake: PropTypes.bool,
  withoutAside: PropTypes.bool,
  embeddedComponent: PropTypes.any,
};

GroupSelector.defaultProps = {
  useFake: false,
  withoutAside: false,
};

const ExtendedGroupSelector = withTranslation(["GroupSelector", "Common"])(
  GroupSelector
);

export default (props) => (
  <I18nextProvider i18n={i18n}>
    <ExtendedGroupSelector {...props} />
  </I18nextProvider>
);

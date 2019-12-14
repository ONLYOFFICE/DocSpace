import React from "react";
import PropTypes from "prop-types";
import AdvancedSelector from "../advanced-selector2";
import { getGroupList } from "../../api/groups";

class GroupSelector extends React.Component {
    constructor(props) {
        super(props);

        const { isOpen } = props;
        this.state = this.getDefaultState(isOpen, []);
      }
    
      componentDidMount() {
        getGroupList(this.props.useFake)
          .then((groups) => this.setState({groups: this.convertGroups(groups)}))
          .catch((error) => console.log(error));
      }

      componentDidUpdate(prevProps) {
        if(this.props.isOpen !== prevProps.isOpen)
          this.setState({ isOpen: this.props.isOpen });
      }

      convertGroups = groups => {
        return groups
          ? groups.map(g => {
              return {
                key: g.id,
                label: g.name,
                total: 0
              };
            })
          : [];
      };

      loadNextPage = ({ startIndex, searchValue }) => {
        console.log(
          `loadNextPage(startIndex=${startIndex}, searchValue="${searchValue}")`
        );

        this.setState({ isNextPageLoading: true }, () => {

          getGroupList(this.props.useFake)
            .then((groups) => {

              const newOptions = this.convertGroups(groups);

              this.setState({
                hasNextPage: false,
                isNextPageLoading: false,
                options: newOptions
              });
            })
            .catch((error) => console.log(error));
        });
      };

      getDefaultState = (isOpen, groups) => {
        return {
          isOpen: isOpen,
          groups,
          hasNextPage: true,
          isNextPageLoading: false
        };
      };

    render() {
        const {
            isOpen,
            groups,
            selectedOptions,
            hasNextPage,
            isNextPageLoading
          } = this.state;

        const { 
            isMultiSelect,
            isDisabled,
            onSelect
        } = this.props;

        return (
        <AdvancedSelector
            options={groups}
            hasNextPage={hasNextPage}
            isNextPageLoading={isNextPageLoading}
            loadNextPage={this.loadNextPage}
            size={"compact"}
            displayType={"auto"}
            selectedOptions={selectedOptions}
            isOpen={isOpen}
            isMultiSelect={isMultiSelect}
            isDisabled={isDisabled}
            searchPlaceHolderLabel={"Search"}
            selectButtonLabel={"Add departments"}
            selectAllLabel={"Select all"}
            groupsHeaderLabel={"Groups"}
            emptySearchOptionsLabel={"There are no departments with such name"}
            emptyOptionsLabel={"There are no departments"}
            loadingLabel={'Loading... Please wait...'}
            onSelect={onSelect}
            onSearchChanged={value => {
              //action("onSearchChanged")(value);
              console.log("Search group", value);
              this.setState({ options: [], hasNextPage: true });
            }}
            onCancel={() => {
              this.setState({ isOpen: false });
            }}
      />);
    }
}

GroupSelector.propTypes = {
  onSelect: PropTypes.func,
  useFake: PropTypes.bool,
}

GroupSelector.defaultProps = {
  useFake: false
}

export default GroupSelector;
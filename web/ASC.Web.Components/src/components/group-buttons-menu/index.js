import React from 'react'
import styled from 'styled-components'
import PropTypes from 'prop-types'
import GroupButton from '../group-button'
import DropDownItem from '../drop-down-item'
import Checkbox from '../checkbox'

const StyledGroupButtonsMenu = styled.div`
    position: sticky;
    top: 0;
    background: #FFFFFF;
    box-shadow: 0px 2px 18px rgba(0, 0, 0, 0.100306);
    height: 56px;
    list-style: none;
    padding: 0 18px 19px 0;
    width: 100%;
    white-space: nowrap;
    display: ${state => state.visible ? 'block' : 'none'};
    z-index: 350;
`;

const CloseButton = styled.div`
    position: absolute;
    right: 20px;
    top: 20px;
    width: 20px;
    height: 20px;

    &:hover{
        cursor: pointer;
    }

    &:before, &:after {
        position: absolute;
        left: 15px;
        content: ' ';
        height: 20px;
        width: 1px;
        background-color: #D8D8D8;
    }

    &:before {
        transform: rotate(45deg);
    }

    &:after {
        transform: rotate(-45deg);
    }
`;

const StyledCheckbox = styled.div`
    display: inline-block;
    margin-left: 16px;
    vertical-align: middle;

    & > * {
        margin: 0px;
    }
`;

class GroupButtonsMenu extends React.PureComponent {

    constructor(props) {
        super(props);

        this.state = {
            priorityItems: props.menuItems,
            moreItems: [],
            visible: true
        }

        this.fullMenuArray = this.props.menuItems;
        this.checkBox = this.props.checkBox;

        this.updateMenu = this.updateMenu.bind(this);
        this.howManyItemsInMenuArray = this.howManyItemsInMenuArray.bind(this);
    }

    componentDidMount() {
        this.widthsArray = Array.from(document.getElementById("groupMenu").children).map(item => item.getBoundingClientRect().width);

        window.addEventListener('resize', _.throttle(this.updateMenu), 100);
        this.updateMenu();
    }

    componentDidUpdate(prevProps) {
        if (this.props.visible !== prevProps.visible) {
            this.setState({ visible: this.props.visible });
        }
    };

    howManyItemsInMenuArray = (array, outerWidth, initialWidth, minimumNumberInNav) => {
        let total = (initialWidth + 180);
        for (let i = 0; i < array.length; i++) {
            if (total + array[i] > outerWidth) {
                return i < minimumNumberInNav ? minimumNumberInNav : i;
            } else {
                total += array[i];
            }
        }
    };

    updateMenu = () => {
        this.outerWidth = document.getElementById("groupMenuOuter") ? document.getElementById("groupMenuOuter").getBoundingClientRect().width : 0;
        this.moreMenu = document.getElementById("moreMenu") ? document.getElementById("moreMenu").getBoundingClientRect().width : 0;

        const arrayAmount = this.howManyItemsInMenuArray(this.widthsArray, this.outerWidth, this.moreMenu, 1);
        const navItemsCopy = this.fullMenuArray;
        const priorityItems = navItemsCopy.slice(0, arrayAmount);

        this.setState({
            priorityItems: priorityItems,
            moreItems: priorityItems.length !== navItemsCopy.length ? navItemsCopy.slice(arrayAmount, navItemsCopy.length) : []
        });
    };

    componentWillUnmount() {
        window.removeEventListener('resize', this.updateMenu());
    }


    render() {
        console.log("GroupButtonsMenu render");
        const closeMenu = (e) => {
            this.setState({ visible: false });
            this.props.onClose && this.props.onClose(e);
        };

        return (
            <StyledGroupButtonsMenu id="groupMenuOuter" visible={this.state.visible} {...this.state}>
                {this.props.hasOwnProperty("checked") &&
                    <StyledCheckbox>
                        <Checkbox isChecked={this.props.checked} isIndeterminate={this.props.isIndeterminate} onChange={(e) => {
                            this.props.onChange && this.props.onChange(e.target.checked);
                        }} />
                    </StyledCheckbox>
                }
                <div id="groupMenu" style={{ display: 'inline-block' }}>
                    {this.state.priorityItems.map((item, i) =>
                        <GroupButton key={`navItem-${i}`}
                            label={item.label}
                            isDropdown={item.isDropdown}
                            isSeparator={item.isSeparator}
                            fontWeight={item.fontWeight}
                            onClick={(e) => {
                                item.onClick(e);
                                closeMenu(e);
                            }}>
                            {item.children}
                        </GroupButton>
                    )}
                </div>
                {this.state.moreItems.length > 0 &&
                    <GroupButton id="moreMenu" isDropdown label={this.props.moreLabel}>
                        {this.state.moreItems.map((item, i) =>
                            <DropDownItem
                                key={`moreNavItem-${i}`}
                                label={item.label}
                                onClick={(e) => {
                                    item.onClick(e);
                                    closeMenu(e);
                                }}
                            />
                        )}
                    </GroupButton>
                }
                <CloseButton title={this.props.closeTitle} onClick={closeMenu} />
            </StyledGroupButtonsMenu>
        );
    }
}

GroupButtonsMenu.propTypes = {
    onClick: PropTypes.func,
    onClose: PropTypes.func,
    onChange: PropTypes.func,
    menuItems: PropTypes.array,
    checked: PropTypes.bool,
    visible: PropTypes.bool,
    moreLabel: PropTypes.string,
    closeTitle: PropTypes.string
}

export default GroupButtonsMenu;
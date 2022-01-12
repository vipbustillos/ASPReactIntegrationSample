import { useEffect, useState, useRef } from "react";
import { ProSidebar, Menu, MenuItem, SubMenu } from "react-pro-sidebar";
import { Navbar, Container, Nav } from "react-bootstrap";
import { IAppConfig, IMenuItem } from "./IAppConfig";

require("react-pro-sidebar/dist/css/styles.css");

export function NavBars(props: IAppConfig) {
  const [collapsed, setCollapsed] = useState(props.leftMenuCollapsed);
  const [headerHeight, setHeaderHeight] = useState(0);
  const { contentDivID, navWidth, navCollapsedWidth, menuDivID } = props;
  let headerDiv = useRef<any>(null);

  const [dimensions, setDimensions] = useState({
    width: window.innerWidth,
    height: window.innerHeight,
  });

  useEffect(() => {
    if (headerDiv) setHeaderHeight(headerDiv?.current?.clientHeight || 0);
  });

  useEffect(() => {
    const handleResize = () => {
      console.log(dimensions.width);
      setDimensions({
        width: window.innerWidth,
        height: window.innerHeight,
      });
      window.addEventListener("load", handleResize, false);
      window.addEventListener("resize", handleResize, false);
    };
  });

  const renderTopNavItem = (menu: IMenuItem) => {
    return (
      <Nav.Link href={menu.url} className={menu.active ? "active" : ""}>
        {menu.iconName && <i className={`fas fa-${menu.iconName}`} />}
        {menu.title}
        {menu.childItems &&
          menu.childItems.map((childItem) => renderTopNavItem(childItem))}
      </Nav.Link>
    );
  };

  const renderLefNavItem = (menu: IMenuItem) => {
    return (
      <>
        {(menu.childItems || []).length == 0 && (
          <MenuItem
            active={menu.active}
            icon={
              menu.iconName && <i className={`fas fa-${menu.iconName}`}></i>
            }
            className={menu.active ? "active" : ""}
          >
            <a href={menu.url}>{menu.title}</a>
          </MenuItem>
        )}
        {(menu.childItems || []).length > 0 && (
          <SubMenu
            title="Components"
            open={menu.active}
            className={menu.active ? "active" : ""}
            icon={
              menu.iconName && <i className={`fas fa-${menu.iconName}`}></i>
            }
          >
            {(menu.childItems || []).map((childmenu) =>
              renderLefNavItem(childmenu)
            )}
          </SubMenu>
        )}
      </>
    );
  };

  const setPageContentPadding = (collapsedValue: boolean, animate: boolean) => {
    const pageContent = document.getElementById(contentDivID);
    if (!collapsedValue) {
      if (pageContent)
        pageContent.className = `navMoveToCollapsed${
          animate ? "" : "-no-animate"
        }`;
    } else {
      if (pageContent)
        pageContent.className = `navMoveToNotCollapsed${
          animate ? "" : "-no-animate"
        }`;
    }
  };

  const topMenu: IMenuItem[] = props.menu?.top || [];
  const leftMenu: IMenuItem[] = props.menu?.left || [];

  return (
    <>
      <style>
        {`
          #${menuDivID} {
            position: absolute;
            height: calc(100% - ${headerHeight}px);
          }
          
          #${contentDivID} {
            padding-top: ${headerHeight}px;
            padding-left:${collapsed ? navCollapsedWidth : navWidth}px;
          }

          .navMoveToCollapsed
          {
            padding-left:${navWidth}px;
            transition: 0.15s all ease;
          }
          .navMoveToNotCollapsed
          {
            padding-left:${navCollapsedWidth}px;
            transition: 0.15s all ease;
          }
          .navMoveToCollapsed-no-animate
          {
            padding-left:${navWidth}px;
          }
          .navMoveToNotCollapsed-no-animate
          {
            padding-left:${navCollapsedWidth}px;
          }

          #${menuDivID} .navbar 
          {
            background: black;
          }

          #${menuDivID} .navbar *
          {
            color:white;
          }
        `}
      </style>
      <div ref={headerDiv}>
        <Navbar bg="dark" style={{ width: dimensions.width }}>
          <a
            style={{ margin: "auto" }}
            onClick={() => {
              let collapsedToggle = !collapsed;
              setCollapsed(collapsedToggle);
              setPageContentPadding(collapsedToggle, true);
            }}
          >
            <i className="fas fa-bars"></i>
          </a>
          <Container>
            <Nav className="me-auto">
              {topMenu.map((menuItem) => renderTopNavItem(menuItem))}
            </Nav>
          </Container>
        </Navbar>
      </div>
      <ProSidebar
        collapsed={collapsed}
        collapsedWidth={navCollapsedWidth}
        width={navWidth}
        className={"side-bar"}
      >
        <Menu iconShape="square">
          {leftMenu.map((menuItem) => renderLefNavItem(menuItem))}
        </Menu>
      </ProSidebar>
    </>
  );
}

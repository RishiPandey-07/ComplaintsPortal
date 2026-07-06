# IT Services Complaints & Requests Portal - User Manual

Welcome to your new enterprise-grade **Complaints & Requests Portal**! This guide will explain how to operate the system from top to bottom.

---

## 1. Initial Setup (Administrator)
When you first deploy the application, you need to configure the core master data. Log in using an account that has the `Administrator` or `OIC IT` role.

### A. Divisions
1. Navigate to **Administration -> Divisions**.
2. Click **+ Add Division**. 
3. Enter the Division Code (e.g., `HRD`) and Name (e.g., `Human Resources Division`).
> [!NOTE]
> The exact spelling of the Division Name must match the `DIVNAME` column in your Oracle `hrdata.empdetails` table. This is how the system automatically figures out which division an employee belongs to!

### B. Roles
1. Navigate to **Administration -> Roles**.
2. Click **+ Add Role**.
3. Create roles like "Group Director", "Associate Director", or "OIC Cleaning".
4. **CRITICAL**: If a role is specific to a division (like a GD), set `Requires Division` to **Yes**. If the role oversees the entire organization (like an OIC IT), set `Requires Division` to **No**.

### C. Assigning Roles to Employees
1. Navigate to **Administration -> User Role Assignment**.
2. Enter an employee's PCNO to search for them.
3. Select the Role (e.g., GD) and the Division they are responsible for.
4. Click **Assign Role**. The workflow engine will now automatically route tickets from that division to this person.

---

## 2. Building Workflows (Administrator)
The true power of this portal is the Dynamic Workflow Engine. You can create completely different approval chains for different types of requests.

### A. Services & Request Types
1. Navigate to **Administration -> Services**. Create a master category (e.g., "IT Services" or "Estate Services").
2. Navigate to **Administration -> Request Types**. Create specific types (e.g., "Hardware Issue" or "Software Installation") under a Service.
3. If a request requires approval, set **Is Flow Based** to **Yes**.

### B. The Workflow Builder
1. Navigate to **Administration -> Workflow Builder**.
2. Select your Request Type (e.g., "Software Installation").
3. Drag and drop roles into the timeline to build the chain. For example:
   * **Stage 1**: Group Director (GD)
   * **Stage 2**: Associate Director (AD)
   * **Stage 3**: OIC IT
4. Click **Save Workflow**. Any new "Software Installation" requests will instantly follow this exact path.

### C. Dynamic Form Builder
1. Navigate to **Administration -> Request Types**.
2. Click the **Fields** button next to a Request Type.
3. Add custom fields that employees must fill out when selecting this request. (e.g., "Which software do you need?").
4. Check **Is Mandatory** if you want to force the employee to fill it out.

---

## 3. Employee Experience
### Submitting a Request
1. Employees log in and click **Employee -> New Request**.
2. They select a Service and Request Type. The custom fields you built will magically appear.
3. They will see a **Visual Preview** of exactly who will need to approve their request.
4. They can upload an attachment and submit.

### Tracking a Request
1. Employees click **Employee -> My Dashboard** to see a quick summary of their active tickets.
2. They click **My Requests** to see a full list. 
3. Clicking **View** on any request shows the **Approval Timeline** so they can see exactly whose desk it is sitting on.

---

## 4. Sanctioning Officer / Approver Experience
### Approving Tickets
1. Approvers (GDs, ADs, OICs) log in and click **My Dashboard** to see how many tickets are waiting for them.
2. They click **Pending Approvals** in the sidebar.
3. They click **Review** on a ticket.
4. They can review all the data, download the employee's attachment, and read the remarks from previous approvers in the timeline.
5. They type a remark and click **Approve / Forward** or **Reject**. The system automatically emails the next person in line.

### Tech Expert Pool (Final Resolution)
1. Once a ticket finishes the approval chain, it drops into the **Tech Expert -> Request Pool**.
2. Technicians can view the pool and click **Pick Up** to assign a ticket to themselves.
3. Once they fix the issue, they click **Resolve**, enter closing remarks, and the ticket is permanently closed. The employee is notified via email.

---

## 5. Helpful Tips
> [!TIP]
> **Standby Assignments:** If a GD goes on leave, an Admin can go to **Administration -> Standby Assignment**, select the GD, select a temporary replacement, and set a date range. The temporary replacement will instantly get access to the GD's approval queue!

> [!WARNING]
> **Email Notifications:** Emails are sent automatically in the background. Ensure your `Web.config` has the correct `SMTPHost` and `SMTPPort` so the portal can successfully dispatch emails.
